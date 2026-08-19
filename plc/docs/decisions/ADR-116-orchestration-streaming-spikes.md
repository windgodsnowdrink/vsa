# ADR-116 编排 / 流处理底座 Spike 结论与采纳决策

- **状态（Status）**：Accepted
- **日期（Date）**：2026-08-19
- **关联**：ADR-114（插件优先战略）、ADR-115（两层模块化架构）、优先级 2 基础能力栈
- **Spike 产物**：`spikes/{dapr-spike,csgo-spike,csharpflink-spike}/`（探索性，未提交 git）

## 0. 背景与目标

按用户指令"并行做三个编排/流处理底座的 Spike"，对 ADR-114 列出的 **Dapr / CsGo / CSharpFlink** 三者在
`.NET 11 Preview 7（SDK 11.0.100-preview.7，rollForward=latestFeature）` 下做可行性验证，回答两个问题：

1. 能否在 .NET 11 Preview 上还原 / 构建 / 跑通核心 API？
2. 能否以 `plc-host` 的 **Tier2 基础能力模块**（`ICapabilityModule` + 可选 `ICarterModule`，Scrutor 扫描，ADR-115）形态接入？

Spike 方法：各建最小 POC 工程（net11.0），还原+构建，跑通核心 API，给出 verdict 与集成草图。

## 1. 决策摘要

| 底座 | Verdict（net11 兼容） | 许可 | 是否采用 |  adopted role |
|------|----------------------|------|----------|---------------|
| **Dapr** (.NET SDK 1.18.5) | Compatible-with-warnings（复用 net10 资产，无 NETSDK1138/NU1701） | MIT | **可选 opt-in Tier2** | AIOT 事件总线(Pub/Sub)+设备会话(Actors) 试点 |
| **CsGo** (HAM-2015/CsGo) | Compatible-with-retargeting（net11.0-windows） | **无 LICENSE（GitHub 404）** | **不采纳（Blocked）** | 仅作范式参考（select/定时器语义） |
| **CSharpFlink** (wxzz/CSharpFlink) | Compatible-with-retargeting（net5.0→net11.0） | Apache-2.0 | **暂缓（Deferred）** | 仅作范式参考（窗口/重算/分布式） |

## 2. Dapr — 可选 opt-in Tier2 能力模块

- **兼容结论**：`Dapr.Client` / `Dapr.AspNetCore` / `Dapr.Workflow` 1.18.5，包内 TFM 仅 net8/9/10；在 net11 上**直接复用 net10 资产，无 TFM 回滚警告**。restore≈8s、build≈2s、0 错误。POC 中 `DaprClientBuilder().Build()` 成功构造，`GetMetadataAsync/SaveStateAsync` 在无 sidecar 时如期抛 `DaprException` —— SDK 形态正确，**运行时依赖 Dapr sidecar**。
- **集成草图**（已编译通过，`DaprCapabilitySketch.cs`）：
  ```csharp
  public sealed class DaprCapability : ICapabilityModule, ICarterModule {
      public string Id => "dapr";
      public void RegisterServices(IServiceCollection s){ s.AddDaprClient(); s.AddDaprWorkflow(); }
      public void AddRoutes(IEndpointRouteBuilder app){ /* /dapr/subscribe 等 */ }
  }
  ```
- **Building Blocks → PLC-IoT 映射**：Pub/Sub→AIOT 事件总线（遥测上行/指令下行）；State→设备影子/会话状态；Virtual Actors→设备会话（每设备一 actor，天然并发隔离）；Workflow→采集→规则→告警→下发编排。Service Invocation 1.18 已标记 Obsolete，改用原生 HTTP/gRPC。
- **风险**：(1) net11 原生资产未发布，当前依赖 net10 回退（非官方支持组合，但无警告）；(2) **sidecar 运维代价**——k8s 注入 daprd+组件 YAML，单机需随进程拉起 sidecar；(3) 引入 gRPC/Protobuf/多包依赖面；(4) runtime 需与 SDK 配套（≥1.18）。
- **决策**：**可选 Tier2，不进核心**。待 AIOT 事件总线需求明确后，先在 Pub/Sub + Actors 试点；暂缓 Service Invocation、Secrets/Configuration。

## 3. CsGo — 不采纳（许可与维护风险）

- **兼容结论**：无 NuGet 包、无 Release；**无 LICENSE 文件（GitHub API 404，法律上不可采用）**；最近提交 2021-07，已停更。重写为 SDK 风格 csproj 并定目标 `net11.0-windows`（因 `control_strand` 耦合 WinForms，强制 Windows-only），补 `System.IO.Ports` 后 **0 警告 / 0 错误**，产出 CsGo.dll(481KB)。POC 生产者-消费者（`chan<int>` send/recv/close）跑通。
- **增量价值 vs 我们规划的 `System.Threading.Channels`+Polly**：`select` 多路复用、`generator.children` 树状取消、内置高精度定时器、协程式 goroutine 调度语义——Channel+Polly 不直接提供。
- **代价**：自研调度器 vs 线程池、必须 Windows、依赖一个停更且无许可的 ~20K 行代码库，维护风险高。
- **决策**：**不引入**。MQTT + `System.Threading.Channels` + Polly 已覆盖队列/弹性需求；若确需 select 语义，自行小范围实现而非依赖该库。

## 4. CSharpFlink — 暂缓（停滞 + 老旧依赖）

- **兼容结论**：Apache-2.0（许可可用）；net5.0，2020-12 后停更，无 NuGet 包（仅源码，本次经 Gitee 镜像拉取）。Core 工程改两处：`net5.0→net11.0` + `<UseVisualBasic>true</UseVisualBasic>`（`SinkFunction.cs` 误引 VB）。依赖全为 2020 版：Confluent.Kafka 1.5.2、DotNetty 0.6.0、Hprose.RPC 3.0.18、Roslyn 3.7.0、Newtonsoft 12.0.3（**NU1903 高危漏洞**）、protobuf-net 3.0.52。`dotnet build` **0 错误**，但 Master 强制监听双 TCP 端口（:7007 + :8007 Hprose），边缘单机多余。POC：source(温度遥测)→5s 窗口 Avg/Max + map/filter 越限告警→ConsoleSink，3 个窗口正确聚合，EXIT=0。
- **增量价值 vs Channel+Polly**：时间窗/聚合链、延迟窗口重算、Roslyn 表达式告警、主从分布式。
- **代价**：停滞 5 年、net11 兼容靠重定目标且未经上游验证；Hprose/DotNetty 老旧；Newtonsoft 已知漏洞；Master 双端口。
- **决策**：**暂缓引入**。IoT 边缘优先 Channel+Polly 轻量管线；仅当确需"延迟窗口重算 / 分布式主从 / 表达式告警"时参考其范式，不自建。

## 5. 总体决策（对 ADR-114 的收敛修正）

ADR-114 将 **Dapr / CsGo / CSharpFlink** 列为三者都要。经 Spike 收敛为：

- **Dapr → 保留为可选 Tier2 能力模块**（opt-in，待 AIOT 事件总线需求落地时接入）。
- **CsGo → 从采纳清单删除**（无许可、停更、Windows-only），仅作范式参考。
- **CSharpFlink → 从采纳清单降级为参考范式**（停滞、老旧依赖、漏洞），不自建流引擎。

即"编排/流处理底座"最终只采纳 **Dapr（按需）+ 自研 Channel+Polly 轻量管线** 两类，CsGo/CSharpFlink 不进入生产依赖图。

## 6. 优先级 2 后续接入顺序建议（仍遵循 ADR-115 "一个类"模式）

| 顺序 | 能力 | 理由 | 风险 |
|------|------|------|------|
| 1 | **HybridCache**（Microsoft.Extensions.Caching.Hybrid，.NET 9+ 首发） | 一等公民、net11 原生、设备影子/配置缓存直接受益 | 低 |
| 2 | **ScottPlot**（图表） | 自包含、HMI/看板可视化，零外部依赖 | 低 |
| 3 | **OpenTelemetry + Aspire** | 一等公民可观测性，支撑"可靠性"品质，net11 兼容 | 中（需 collector/仪表盘） |
| 4 | **CommunityToolkit**（Diagnostics/Mvvm） | 未来 HMI 客户端复用，轻量 | 低 |
| 5 | **Foundatio**（异常/缓存/队列抽象） | 与已有 Polly/Channel 有重叠，评估后取舍 | 中（功能重叠） |
| 6 | **Native AOT** | 编译期全程序约束，作为宿主硬化收尾 Spike | 高（反射/动态加载需 `[DynamicDependency]`） |
| opt-in | **Dapr** | 仅当 AIOT 事件总线/设备会话需求明确 | 中（sidecar 运维） |

> Oqtane 思路已在 ADR-114 拍板为"自研轻量宿主"，本次以 Scrutor+Carter 落地（ADR-115），无需再引包。

## 7. 风险与跟进

- Spike 产物 `spikes/` 为探索性代码，**不纳入 git 提交**；如需保留请单独确认，否则可删除。
- Dapr 的 net11 组合为非官方支持态，接入前应在 net11 GA 后复测或等待官方 net11 资产。
- CsGo/CSharpFlink 的"参考范式"结论记录在案，避免后续重复评估。
