# Dapr .NET SDK — Spike 报告 (plc-host / net11.0)

## ① Verdict
**Compatible-with-warnings.**
Dapr 1.18.5 (.NET 8/9/10) 在 .NET 11 Preview 7 上可正常 `restore`/`build`/构造 `DaprClient`，
**未出现 `NETSDK1138` 也未出现 TFM 回滚 `NU1701`**（net11 直接复用 net10 资产，无警告）。
Dapr 官方已声明计划在 .NET 8/9 EOL 后转向支持 .NET 10 与 .NET 11，net11 原生资产只是时间问题。

## ② 包/版本 与 还原+构建结果
- 包源：`https://api.nuget.org/v3/index.json`（直连可达）
- `Dapr.Client` 1.18.5、`Dapr.AspNetCore` 1.18.5、`Dapr.Workflow` 1.18.5
- TFM：包内仅 net8.0/net9.0/net10.0（无 net11.0）
- `dotnet restore` ≈ 7.94s，成功；`dotnet build` ≈ 2.18s，**0 错误**
- 唯一警告：`NU1510`(Microsoft.Extensions.Hosting 自动可用，可删引用)、`CA1707`(命名空间下划线，本 spike 特有)、`NETSDK1057`(.NET 预览版通用提示)。**无 NETSDK1138 / 无 NU1701**。
- SDK：11.0.100-preview.7（受根 `global.json` rollForward=latestFeature 影响）

## ③ POC 结果
`new DaprClientBuilder().Build()` 在 net11 上成功构造；
`GetMetadataAsync()` 与 `SaveStateAsync()` 在无 sidecar 时如期抛出 `DaprException`
("the Dapr endpoint indicated a failure")。结论：**SDK 形态正确、可编译可构造；运行时依赖 Dapr sidecar**。

## ④ 集成草图（见 DaprCapabilitySketch.cs，已编译通过）
```csharp
public sealed class DaprCapability : ICapabilityModule, ICarterModule {
    public string Id => "dapr";
    public void RegisterServices(IServiceCollection s){ s.AddDaprClient(); s.AddDaprWorkflow(); }
    public void AddRoutes(IEndpointRouteBuilder app){ /* /dapr/subscribe 等 */ }
}
```
对应 Building Blocks → PLC-IoT 需求：
- **Pub/Sub** → AIOT 事件总线（设备遥测上行 / 指令下行）
- **State** → 设备影子 / 会话状态持久化
- **Virtual Actors** → 设备会话 actor（每设备一个，天然并发隔离）
- **Workflow** → 编排工作流（采集→规则→告警→下发）
- Service Invocation → 模块/服务间调用（可由原生 HTTP/gRPC 替代，1.18 已标记 Obsolete）

## ⑤ 风险与建议
- **建议角色：可选（opt-in Tier2 能力模块）**，不进核心；先在 AIOT 事件总线(Pub/Sub)+设备会话(Actors)试点。
- **主要风险**：(1) net11 原生资产未发布 → 依赖 net10 回退（当前无警告，但属非官方支持组合）；(2) sidecar 运维代价——k8s 需注入 daprd + 组件 YAML，单机 dice 部署需随进程拉起 sidecar；(3) 引入 gRPC/Protobuf/多包依赖面；(4) Dapr runtime 需与 SDK 版本配套（1.18 SDK 建议 runtime ≥1.18）。
- **暂缓**：Service Invocation（官方已不推荐，用原生 HTTP）、Secrets/Configuration 除非确需。
