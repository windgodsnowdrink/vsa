# SNET 生产组件反编译恢复 + PLC 网关集成蓝图

> 目标：把 `docs/snet` 下 51 个 `Snet.*` 生产 DLL（net8.0 / net10.0）反编译还原为源代码，并基于它集成「File-based App + 插件化 + Disruptor 流水线 + vs-threading + StreamJsonRpc + IoTClient 风格 PLC 协议」，实现多协议 PLC 采集与「消息记忆」。

## 0. 反编译结果（已完成）

使用 `ilspycmd 11.0.0` 将 51 个 `docs/snet/*/lib/net8.0/*.dll` 逐一反编译，输出到 `docs/snet/decompiled/<Assembly>/<Assembly>.decompiled.cs`（单文件合并）。

| 分类 | 程序集 | 行数 | 关键类型 |
|------|--------|------|----------|
| 核心 | Snet.Core | ~23.6k | `DaqAbstract<O,D>`、`PluginOperate`、`ChannelOperate<T>`、`SubscribeService<T>`、`ShareCacheOperate`、`ProcessCacheOperate`、`TcpClientOperate`/`UdpClientOperate`/`SerialOperate`/`WsClientOperate`/`HttpClientOperate` |
| 模型 | Snet.Model | ~4.3k | `IDaq`、`IRead`、`IWrite`、`ISubscribe`、`ICommunication`、`IEvent`、`Address`、各 `Operate` 的 `Basics` 参数 |
| 驱动协议 | 41 个 `Snet.Xxx` | 各 1.5k–3.5k | `SiemensOperate`、`MitsubishiOperate`、`OmronOperate`、`ModbusOperate`、`AllenBradleyOperate`、`BeckhoffOperate`、`FatekOperate`、`FujiOperate`、`KeyenceOperate`、`PanasonicOperate`、`OpcUaClientOperate`、`DeltaOperate`、`FanucOperate`、`GE`/`Inovance`/`Invt`/`Yaskawa`/`Yokogawa`/`Toyota`/`Turck`/`Vigor`/`WeCon`/`XinJE`/`Yamatake`/`YuDian`/`LSis`/`Cimon`/`Kossi`/`RKC`/`MegMeet`/`Freedom`/`PQDIF`/`Sim`/`TEP` 等 |
| 消息/缓存 | Snet.Log / Snet.DB / Snet.Mqtt / Snet.Kafka / Snet.RabbitMQ / Snet.Redis / Snet.NetMQ / Snet.Rpc | — | `MqOperate`（生产者/消费者）、DB 持久化、`ShareCache` 内存缓存（即「消息记忆」落点） |
| 工具 | Snet.Utility / Snet.Driver | — | `IReadWriteDevice`/`IReadWriteNet`（底层寄存器读写）、字节转换、压缩 |

**关键发现**：这套 `Snet` 框架本身就是「Daq 抽象 + 插件 + 通道 + 缓存」的完整工业采集框架，与你列出的 8 项需求几乎一一对应——它已经实现了 shunnet/Daq 的 Daq 模型、自带插件机制（`PluginOperate`）、自带通道/订阅（`ChannelOperate`/`SubscribeService`），以及 40+ PLC 协议驱动。「消息记忆」对应 `ShareCacheOperate` / `ProcessCacheOperate` 的本地缓存与历史留存。

## 1. 八项需求 → 落地映射

| # | 用户需求 | 在 SNET / 外部库中的落地 |
|---|----------|---------------------------|
| 1 | 反编译 + 实现各种 PLC 协议与消息记忆 | 已完成反编译；协议由 `Snet.XxxOperate : DaqAbstract` 提供；消息记忆 = `ShareCacheOperate`/`ProcessCacheOperate` + `Snet.DB` 持久化 |
| 2 | 参照 csharp-looks-like-go 实现 file-based app | 在 `plc-host` 采用「单文件入口 + 配置即代码」：用 `app.manifest.json` 声明设备/插件/流水线，宿主启动即从文件装配，不依赖 IDE 工程嵌套 |
| 3 | 参照 shunnet/Daq 实现插件 | SNET 已有 `DaqAbstract`/`IDaq`（IOn/IOff/IRead/IWrite/ISubscribe）；新增插件只需 `XxxOperate : DaqAbstract<XxxOperate, XxxData.Basics>` |
| 4 | 引入 Disruptor-net 做协议生产/消费（数据/日志/消息） | 用 `Disruptor` 环形缓冲区替换/增强 `ChannelOperate<T>`：采集线程 produce 原始帧，消费者线程做解析/落库/转发；日志与消息走同一 RingBuffer 分流 |
| 5 | 引入 DotNetCorePlugins 实现插件 | 用 `McMaster.NETCore.Plugins` 按目录加载 `plugins/*.dll`，取代现有 ALC 方案；每个插件包 = 一组 `DaqAbstract` 驱动 + 元数据；契约走共享 `Plc.Plugins.Contracts` |
| 6 | 引入 vs-threading 实现高性能 | `Microsoft.VisualStudio.Threading` 的 `AsyncSemaphore`、`AsyncReaderWriterLock`、`JoinableTaskFactory` 用于并发连接池、背压、避免死锁 |
| 7 | 引入 StreamJsonRpc 实现组件间 RPC | 宿主 ↔ 插件进程 / 网关 ↔ 前端 用 `StreamJsonRpc` 在 NamedPipe/WebSocket 上做 JSON-RPC；暴露 `Read`/`Write`/`Subscribe` 方法契约 |
| 8 | 参照 IoTClient 实现 PLC 协议 | IoTClient（zhaopeiym）是另一套 PLC 库；作为对照/兜底驱动源，优先用已恢复的 SNET 41 驱动，IoTClient 可作为补充（如缺少的协议）或替换参考 |

## 2. 已有契约（现有 `src/Plc.Plugins.Contracts`）

项目已有 `IPlugin` / `IDeviceProtocol` / `IDeviceSession` / `IDeviceCatalog`。建议：保留这些契约作为「宿主↔插件」边界，内部用适配器把 `Snet.IDaq` 包成 `IDeviceSession`（读/写寄存器）。这样既不抛弃既有 ALC 成果，又能用 DotNetCorePlugins 加载。

## 3. 推荐分阶段落地（可逐项自动化/逐步执行）

**阶段 A — 宿主骨架（file-based）**
- 新建/收敛 `src/plc-host`（或新 `plc-gateway`），`Program.cs` 只读 `app.manifest.json` → 装配插件目录、连接、流水线。
- 引用：`Snet.Core.dll`、`Snet.Model.dll`、`Snet.Driver.dll` + 所需协议 DLL（net8.0），NuGet 补充 `Microsoft.Extensions.*`、`System.IO.Ports`（Snet.Core 依赖详见各自 `.nuspec`）。

**阶段 B — 插件加载切换为 DotNetCorePlugins**
- `PluginManager` 用 `PluginLoader.CreateFromAssemblyFile(...)` 按 `plugins/` 加载；每个插件 dll 引用共享 `Plc.Plugins.Contracts`。
- 适配器：`SnetDaqAdapter : IDeviceProtocol`，把 `IDaq.ReadAsync(address)` 映射为 `IDeviceSession.ReadAsync`。

**阶段 C — Disruptor 流水线**
- `RingBuffer<RawFrame>`：采集 → 解析 → 落库/转发 三段消费者；日志事件也入 RingBuffer 单独消费者（「消息记忆」）。

**阶段 D — vs-threading + StreamJsonRpc**
- `JoinableTaskFactory` 管理生命周期；`StreamJsonRpc` 在 IPC 通道暴露 RPC。

**阶段 E — IoTClient 补充**（可选）：把 IoTClient 缺的协议以驱动形式接入同一 `IDeviceProtocol` 体系。

## 4. 风险与未决项

- **依赖**：每个 `Snet.*` 程序集还依赖 `Snet.Driver`/`Snet.Model`/`Snet.Core` 及 `Microsoft.Extensions.*`；引用 DLL 时需把全部依赖 DLL 一并拷贝到输出目录（或走 NuGet 还原对应版本 26.235.x）。
- **版本**：DLL 是 net8.0/net10.0，宿主若用 net11 需兼容（net8 程序集可被 net11 引用，运行时 OK）。
- **反编译源码不可直接编译**（单文件、部分语法需整理）；建议**引用二进制**（已验证依赖可控）而非重新编译源码。
- ALC 与 DotNetCorePlugins 二选一：本方案建议**改用 DotNetCorePlugins**（按用户第 5 项），旧 ALC 契约保留为适配器目标。

## 5. 交付物

- `docs/snet/decompiled/**` — 51 个程序集的反编译源码（可查阅）
- `docs/snet/RECOVERY_AND_INTEGRATION.md` — 本文
- 后续（需你确认是否进入 Craft 模式动手）：`plc-gateway` 骨架 + 适配器 + 流水线，可运行验证 Modbus/Siemens/Mitsubishi 采集。
