# ADR-119 优先级「协议层 + 高性能并发管道」顶层设计

- **状态（Status）**：**Accepted**（2026-10-18 用户锁定决策 ①~④）
- **日期（Date）**：2026-10-18
- **关联**：ADR-114（插件优先战略）、ADR-115（两层模块化）、ADR-117（能力栈）、ADR-118（优先级3 协议插件化）
- **范围**：优先级「可扩展 / 高性能 / 插件化 / 可靠性」中的协议层与并发管道

---

## 0. 现状核查（实证，逐一验证过，非推断）

| 项 | 文档/MEMORY 的说法 | 代码实测 | 证据 |
|---|---|---|---|
| `IProtocolAdapter` | 架构文档 §6.2 已定义、前端 JS 也在用 | **零实现、零消费者**，只存在于 SVG/文档/前端注释 | `docs/*.svg`、`wwwroot/js/app.js:854`，`src/**/*.cs` 无命中 |
| 运行时契约 | 与上式同期 | **真实契约是 `IDeviceProtocol` / `IDeviceSession` / `IDeviceCatalog`** | ADR-118 §1.2；`src/plc-gateway/GatewayHost.cs:26` |
| 协议实现 | 「引入 IoTClient 做设备通信」 | 7 个 ALC 插件**全部裸 `TcpClient` 自研帧**，IoTClient 全仓库零引用 | 各插件 `private TcpClient? _client;` |
| DotNetCorePlugins | 「待引入」 | **已引入**：`PlcGateway.csproj:21` McMaster.NETCore.Plugins 1.1.0；`SnetPluginProbe.csproj:19` 同款 | — |
| disruptor-net | 「待引入」 | **已引入**：`PlcGateway.csproj:22` Disruptor 4.0.0 | — |
| vs-threading | 「待引入」 | **已引入**：`PlcGateway.csproj:23` Microsoft.VisualStudio.Threading 17.11.20 | — |
| vs-streamjsonrpc | 「待引入」 | **已引入**：plc-host 2.25.29（`plc-host.csproj:72`）、plc-gateway/GatewayClient 2.21.69 | — |
| **IoTClient** | 「待引入」 | **确为零引用**（唯一真正缺口） | 全仓库 grep 无命中 |
| snet 生产 DLL | 「反编译用于协议与消息记忆」 | **已落地为 `SnetProtocols` 插件**：直接 `<Reference>` 40+ 个 `Snet.*.dll`（Core/Model/Driver/Log + Modbus/Siemens/Mitsubishi/Omron/AllenBradley/Fatek/Fuji/Keyence/Panasonic/Delta/Fanuc/GE/Inovance/...） | `src/plc-gateway/plugins/SnetProtocols/SnetProtocols.csproj:26-63` |
| 反编译源码编译 | 「反编译 snet DLL 还原源码」 | **保持只参考**：`<Reference>` 指向**二进制** DLL，无任何 `.decompiled.cs` 进入构建 | 同上 |

**结论**：待办清单 5 项中 4 项（DotNetCorePlugins / disruptor / vs-threading / vs-streamjsonrpc）**已在 plc-gateway 落地**，
真正未做的只有 **IoTClient** 与 **plc-host 侧的统一帧格式 + 数据面并发管道**。

---

## 1. 决策 ①（已锁定）：单一 `IDeviceProtocol`，不引入 `IProtocolAdapter`

**用户裁定：单一 `IDeviceProtocol`——不新增接口，把帧能力直接扩进既有契约。**

- **废弃 `IProtocolAdapter` 这一命名**：它从未落地，继续保留只会让文档与代码长期脱节。
- 帧能力归属：
  - 统一帧类型 `DeviceFrame` 作为**数据契约**放在同一共享程序集；
  - 读写入口统一收敛在 `IDeviceSession`（`ReadAsync` / `WriteAsync` 及其批量形态），
    不再另设"适配器层"。
- **命名清理（随本 ADR 一并执行）**：架构文档 / SVG / 前端注释中的 `IProtocolAdapter`
  统一改为 `IDeviceProtocol`，消除"文档里有一套、代码里是另一套"的双轨。
  - 已改：`wwwroot/js/app.js:854` 的提示文案。
  - 待办：`docs/architecture-merged.svg`、`docs/architecture-diagram.svg`、`docs/data-plane-diagram.svg`
    中的节点文案（图形资产，随下次文档修订一并更新）。
- 收敛目标：**只有一套契约**，即 `IDeviceProtocol` + `IDeviceSession` + `IDeviceCatalog` + `DeviceFrame`。

### 1.1 契约扩展方式：默认接口方法（C# 14），不破坏现有实现

`IDeviceSession` 是共享契约，硬扩接口会一次性波及 7 个插件 + 2 个宿主 + verify 测试。
采用 **新增成员 + 默认接口方法（DIM）** 的并存方式：

- 新成员给出**默认实现**（转调既有 `ReadAsync`/`WriteAsync`），现有插件**零改动即可编译**；
- 需要更高性能的插件（如 Modbus 的批量读）自行覆写，按协议逐个优化；
- 旧成员保留，不设 `[Obsolete]` 的破坏性移除，待全部插件覆写后再评估收敛。

```csharp
public interface IDeviceSession : IAsyncDisposable
{
    // —— 既有（保留）——
    Task<byte[]> ReadAsync(ReadRequest request, CancellationToken ct = default);
    Task WriteAsync(WriteRequest request, CancellationToken ct = default);

    // —— 新增（默认实现转调，插件可覆写）——
    Task<IReadOnlyList<DeviceFrame>> ReadBatchAsync(
        IReadOnlyList<ReadRequest> requests, CancellationToken ct = default)
    {
        // 默认：串行转调 ReadAsync，包装为 DeviceFrame
    }
    // 连接复用：默认 false（维持"每次请求新建连接"现状），插件支持时覆写为 true
    bool SupportsReuse => false;
}
```

---

## 2. 决策 ②（已锁定）：IoTClient 走可选后端，先验 Modbus TCP

- 现状 7 个插件 ~1400+ 行自研帧已可跑通并被 `tests/DeviceProtocols.Verify` 依赖，整体替换风险高；
  且 IoTClient 对 BACnet/IP、LoRaWAN 无对应实现。
- 采纳 **可选后端**：自研保持默认；IoTClient 仅作为备选驱动接入，**先只拿 Modbus TCP 一个点做等价性验证**，
  验证通过再谈是否扩展到已覆盖协议。
- 若验证不通过（.NET 11 预览版兼容 / API 差异），**直接放弃引入**，不阻塞本 ADR 其余部分。
- 验证判据：同一批寄存器地址、同一设备，自研路径与 IoTClient 路径的读结果逐字节一致，且长稳无连接泄漏。

---

## 3. 决策 ③（已锁定）：`DeviceFrame` 落在 `Plc.Plugins.Contracts`

- 位置：`src/Plc.Plugins.Contracts/`（与 `IDeviceProtocol` 同驻，保证插件↔宿主类型身份统一）。
- 形态：以 `ReadOnlyMemory<byte>` 承载载荷 + 显式元数据字段（协议 ID、设备 ID、时间戳、方向、序号），
  **不引入序列化框架依赖**，便于后续按协议扩展。
- 与 plc-gateway 现有 `RawFrame` 的关系：`RawFrame` 是 Disruptor 槽位复用的**可变**类；
  `DeviceFrame` 是**不变**契约类型，网关侧可按需适配，**不强制替换** `RawFrame`
  （避免动已验证的环形缓冲槽位复用逻辑）。

---

## 4. 决策 ④（已锁定）：三项都做 —— 数据面 + Disruptor 管道 + vs-threading

**用户裁定：纳入 plc-host 数据面、Disruptor 采集管道、vs-threading 并发原语三项；
本次不做 plc-host 与 plc-gateway 的"双宿主收口"。**

plc-host 当前数据面 = HTTP 请求 → 每次 `CreateSession` 新建 TCP 连接 → 读 → 释放，
**无连接池、无批处理、无背压**。

### 4.1 执行顺序（按依赖关系排，非按重要性）

| 序 | 项 | 理由 |
|---|---|---|
| 1 | **vs-threading 引入 plc-host** | 它是另两项的并发底座（`AsyncSemaphore` 背压、`AsyncLazy` 懒连接、`JoinableTaskFactory`）；网关侧正是用它驱动 Disruptor 发布，已有可复用范式 |
| 2 | **Disruptor 4.0.0 钉齐 + 采集管道** | 依赖 1 的原语；版本必须与 plc-gateway 的 4.0.0 一致，避免两边 API 漂移 |
| 3 | **plc-host 数据面**（会话池 + 批量读 + Polly 弹性） | 会话池/批量读需要 1 的锁与懒加载；Polly 已在 ADR-117 能力栈中，只需接线 |
| 4 | IoTClient 等价性验证（Modbus TCP） | 不阻塞主线，可与 1~3 并行；失败即弃 |

> 若希望先把"每请求新建连接"这个最痛的点止血，**第 3 项可提前到第 1 项之后单独上线**——
> 它本身不依赖 Disruptor。

### 4.2 明确不做

- **不做双宿主收口**：plc-host 与 plc-gateway 都能加载插件、都能读写设备（当前经 NamedPipe 代理），
  权威归属问题本轮不动，沿用现状。
- **不在 AOT 裁剪图内引入反射型插件加载**：沿用 ADR-114/115 的 ALC 边界。

---

## 5. 里程碑

| # | 内容 | 产出 | 依赖 | 状态 |
|---|---|---|---|---|
| M1 | 契约层：`DeviceFrame` + `IDeviceSession` 扩展（批量读 DIM、连接复用标记）+ 命名清理 | `Plc.Plugins.Contracts`、`wwwroot/js/app.js` | — | ✅ 已完成 |
| M2 | vs-threading 引入 plc-host（`AsyncSemaphore`/`AsyncLazy`/`JoinableTaskFactory`） | `plc-host.csproj` + 并发底座 | M1 | 待开工 |
| M3 | Disruptor 4.0.0 钉齐 + 采集管道骨架（环形缓冲） | `plc-host.csproj` + 管道 | M2 | 待开工 |
| M4 | plc-host 数据面：会话池 + 批量读 + Polly 弹性 | `DeviceProtocolsModule` 改造 | M2（可提前） | 待开工 |
| M5 | Modbus TCP 参考实现覆写批量读 + IoTClient 等价性验证 | 插件内新文件 + 验证脚本/结论 | M1（可并行） | 待开工 |
| M6 | 门禁：新契约/管道的构建 + 冒烟脚本纳入 `scripts/` | CI 可对 `net11.0` 构建 | M1~M5 | 待开工 |

### 5.1 M1 落地说明（契约层）

**新增 `src/Plc.Plugins.Contracts/DeviceFrame.cs`**

- `FrameDirection`：`Inbound`（设备 → 宿主）/ `Outbound`（宿主 → 设备）。
- `DeviceFrame`（`sealed record`，不可变）：`ProtocolId` / `DeviceId` / `Timestamp`(UTC `DateTimeOffset`) /
  `Direction` / `Payload`(`ReadOnlyMemory<byte>`) / `Sequence` / `RegisterType` / `ElementType` /
  `StartAddress` / `Count` / `AddressText` / `Elapsed` / `Error`；派生 `IsSuccess`、`Length`。
- 三个工厂：`Read(...)`（入站）、`Write(...)`（出站，无返回载荷）、`Failure(...)`（失败帧）。
- 值语义：`Payload` 按**字节内容**判等（`SequenceEqual`），哈希 `HashCode.AddBytes`，
  避免 `ReadOnlyMemory<T>` 默认引用比较造成的"内容相同却不相等"陷阱。
- 不依赖任何序列化框架：序列化是 MQTT / InfluxDB / Outbox 层的事。

**扩展 `IDeviceSession`（全部为默认接口方法，现有实现零改动）**

| 成员 | 默认实现 | 语义 |
|---|---|---|
| `string ProtocolId => ""` | 空串 | 帧元数据；协议实现应覆写为 `IDeviceProtocol.ProtocolId` |
| `bool SupportsConnectionReuse => false` | false | 宿主数据面据此决定是否入会话池（保守默认：不复用） |
| `Task<IReadOnlyList<DeviceFrame>> ReadBatchAsync(requests, deviceId, ct)` | 串行转调 `ReadAsync` | 与入参**同序同长**；单项失败**不抛**，落为 `Failure` 帧；`OperationCanceledException` 原样上抛 |

> **DIM 使用约束**：默认接口成员**只在接口上可见**，实现类类型的变量看不到它们。
> 宿主/管道代码必须按 `IDeviceSession` 编程（`IDeviceSession s = new ModbusTcpSession(...)`），
> 不可用 `var` 推断为实现类——这是 DIM 的语言固有语义，不是本仓约定。

**命名清理**：`IProtocolAdapter` 在 `src/` 下已清零（`wwwroot/js/app.js`、
`PlcAiot.Abstractions/Devices/IDeviceProtocol.cs` 注释、`PlcAiot.Web/cockpit.html`）。
`docs/*.svg` 中的节点文案留待下一轮文档修订统一替换。

**验证**：`Plc.Plugins.Contracts` / `plc-host`（含 7 个插件）/ `plc-gateway` 全部 **0 错误**构建；
契约行为探针 **26/26 PASS**（DIM 默认路径、覆写路径、部分失败落帧、取消上抛、
`DeviceFrame` 内容判等与哈希、旧 `ReadAsync`/`WriteAsync` 未破坏）。

---

## 6. 风险与缓解

1. **.NET 11 Preview 包冲突**：新增 NuGet 在预览版 SDK 上易触发版本冲突（历史上有 `NU1107` 硬错误先例）。
   缓解：引入前先 `dotnet restore` 验证；版本与 plc-gateway 现存者**钉齐**（Disruptor 4.0.0、VS.Threading 17.11.20）。
2. **契约变更波及面**：`IDeviceSession` 是共享契约，改动需同步 7 个插件 + 2 个宿主 + verify 测试。
   缓解：**默认接口方法**提供默认实现（见 §1.1），现有实现零改动；旧成员保留，不破坏性移除。
3. **Disruptor 版本分裂**：plc-gateway 4.0.0 vs `plc-vsa-demo` 6.0.1，两者 `IEventHandler`/`Dsl` API 不兼容。
   缓解：plc-host 一律钉 **4.0.0**；`plc-vsa-demo` 维持独立，不作为引入依据。
4. **SNET DLL 只参考**：`SnetProtocols` 以 `<Reference>` 引用二进制，反编译源码一律不进构建——新增协议插件沿用此约束。
5. **双宿主未收口（已知债务）**：plc-host 与 plc-gateway 能力重叠，本轮不动，后续单独立项。
