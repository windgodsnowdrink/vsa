# ADR-119 优先级「协议层 + 高性能并发管道」顶层设计

- **状态（Status）**：**Proposed（待用户锁定决策 ①~④ 后转 Accepted）**
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
| DotNetCorePlugins | 「待引入」 | **已引入**：`PlcGateway.csproj:21` McMaster.NETCore.Plugins 1.1.0；`SnetPluginProbe.csproj:19` 同款 | 见下 |
| disruptor-net | 「待引入」 | **已引入**：`PlcGateway.csproj:22` Disruptor 4.0.0 | — |
| vs-threading | 「待引入」 | **已引入**：`PlcGateway.csproj:23` Microsoft.VisualStudio.Threading 17.11.20 | — |
| vs-streamjsonrpc | 「待引入」 | **已引入**：plc-host 2.25.29（`plc-host.csproj:72`）、plc-gateway/GatewayClient 2.21.69 | — |
| **IoTClient** | 「待引入」 | **确为零引用**（唯一真正缺口） | 全仓库 grep 无命中 |
| snet 生产 DLL | 「反编译用于协议与消息记忆」 | **已落地为 `SnetProtocols` 插件**：直接 `<Reference>` 40+ 个 `Snet.*.dll`（Core/Model/Driver/Log + Modbus/Siemens/Mitsubishi/Omron/AllenBradley/Fatek/Fuji/Keyence/Panasonic/Delta/Fanuc/GE/Inovance/...） | `src/plc-gateway/plugins/SnetProtocols/SnetProtocols.csproj:26-63` |
| 反编译源码编译 | 「反编译 snet DLL 还原源码」 | **保持只参考**：`<Reference>` 指向**二进制** DLL，无任何 `.decompiled.cs` 进入构建 | 同上 |

**结论**：待办清单里 5 项中的 4 项（DotNetCorePlugins / disruptor / vs-threading / vs-streamjsonrpc）**已在 plc-gateway 落地**，
真正未做的只有 **IoTClient** 与 **plc-host 侧的统一帧格式 + 数据面并发管道**。

---

## 1. 决策 ①（命名）：不改运行时契约，另立帧级适配器

- `IDeviceProtocol` / `IDeviceSession`（会话级：字节块读写）保持不变，避免一次性动 7 个插件 + 2 个宿主。
- 新增 `IProtocolAdapter` 定位为**帧级适配器**：在 `IDeviceProtocol` 之上负责「原始字节 ↔ 统一帧」的编解码与元数据补全，供
  Disruptor/采集管道/消息记忆消费。
- 前端 `wwwroot/js/app.js:854` 里提到的 `IProtocolAdapter.WriteAsync` 语义与之相容（写命令 → 适配器 → 帧）。
- 收敛目标：`IDeviceProtocol`（传输）+ `IProtocolAdapter`（帧）两层，取代文档中含混的单层描述。

## 2. 决策 ②（IoTClient）：可选后端，不替换自研

- 现状 7 个插件 ~1400+ 行自研帧已可跑通并被 `tests/DeviceProtocols.Verify` 依赖，整体替换风险高；
  且 IoTClient 对 BACnet/IP、LoRaWAN 无对应实现。
- 采纳 **可选后端**：自研保持默认；IoTClient 仅作为备选驱动接入（先 Modbus TCP 一个点做等价性验证，
  验证通过再谈是否扩展到已覆盖协议）。
- 若验证不通过（.NET 11 预览版兼容 / API 差异），直接放弃引入，不阻塞本 ADR 其余部分。

## 3. 决策 ③（统一帧格式）：`DeviceFrame` 落地在共享契约

- 位置：`src/Plc.Plugins.Contracts/`（与 `IDeviceProtocol` 同驻，保证插件↔宿主类型身份统一）。
- 形态：以 `ReadOnlyMemory<byte>` 承载载荷 + 显式元数据字段（协议 ID、设备 ID、时间戳、方向、序号），
  **不引入序列化框架依赖**，便于后续按协议扩展。
- 与 plc-gateway 现有 `RawFrame` 的关系：`RawFrame` 是 Disruptor 槽位复用的可变类；
  `DeviceFrame` 是不变契约类型，网关侧可按需适配，不强制替换。

## 4. 决策 ④（并发管道）：先补 plc-host 数据面，与 plc-gateway 对齐

- plc-host 当前数据面 = HTTP 请求 → 每次 `CreateSession` 新建 TCP 连接 → 读 → 释放，**无连接池、无批处理、无背压**。
- 落地顺序（低风险优先）：
  1. 「稳固基本盘」：会话池/复用 + 批量读 + 连接弹性（Polly，已在能力栈中）。
  2. 「采集管道」：vs-threading 的 `AsyncReaderWriterLock` / `AsyncLazy` 保证线程安全与懒加载（plc-gateway 已验证可用）。
  3. 「环形缓冲」：Disruptor 4.0.0 与 plc-gateway 版本钉齐，避免两边 API 漂移。
- 明确不做：不在 AOT 裁剪图内引入反射型插件加载（沿用 ADR-114/115 的 ALC 边界）。

---

## 5. 里程碑（决策锁定后执行）

| # | 内容 | 产出 |
|---|---|---|
| M1 | 契约层：`IProtocolAdapter` + `DeviceFrame` + 扩展 `IDeviceSession`（批量读、连接复用） | `Plc.Plugins.Contracts` |
| M2 | 协议适配：Modbus TCP 的 `IProtocolAdapter` 参考实现（包自研帧） | 插件内新文件 + 单测 |
| M3 | IoTClient 可选后端等价性验证（Modbus TCP） | 验证脚本/结论；失败即弃 |
| M4 | plc-host 数据面：会话池 + 批量读 + Polly 弹性 | `DeviceProtocolsModule` 改造 |
| M5 | vs-threading 引入 plc-host + Disruptor 版本钉齐（4.0.0） | csproj + 采集管道骨架 |
| M6 | 门禁：新契约/适配器的构建 + 冒烟脚本纳入 `scripts/` | CI 可对 `net11.0` 构建 |

---

## 6. 风险

1. **.NET 11 Preview**：新增 NuGet 在预览版 SDK 上易触发版本冲突（历史上 `NU1107` 硬错误先例），
   引入前先 `dotnet restore` 验证，版本与 plc-gateway 现存者钉齐。
2. **契约变更波及面**：`IDeviceSession` 是共享契约，改动需同步 7 个插件 + 2 个宿主 + verify 测试；
   采用「新增接口 + 旧接口保留」的并存方式，避免破坏性修改。
3. **SNET DLL 只参考**：`SnetProtocols` 以 `<Reference>` 引用二进制，反编译源码一律不进构建——新增协议插件沿用此约束。
