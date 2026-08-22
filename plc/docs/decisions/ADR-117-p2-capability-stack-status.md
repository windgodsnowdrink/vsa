# ADR-117 优先级2 基础能力栈落地状态与采纳清单

- **状态（Status）**：Accepted
- **日期（Date）**：2026-08-22
- **关联**：ADR-114（插件优先战略）、ADR-115（两层模块化架构）、ADR-116（编排/流处理 Spike）、ADR-118（优先级3 PLC 插件化）
- **优先级 2 范围**：基础能力栈（Scrutor/Polly/Carter 已落 ADR-115；本 ADR 收口其余 6+1 项与 Native AOT Spike）。

## 0. 背景与目标

优先级 2 按 ADR-114 拍板、ADR-115 确立的"**一个类**"模块模式（`ICapabilityModule` + 可选 `ICarterModule`，Scrutor 扫描，`AddCapabilityModules()`）逐项接入基础能力。
本 ADR 收口全部 P2 项的最终采纳状态，并记录 Native AOT 硬化 Spike 结论，作为优先级 2 收口依据。

## 1. P2 能力栈最终采纳清单

| 能力 | 包/版本 | 形态 | 状态 | 关键备注 |
|------|---------|------|------|----------|
| **Scrutor** 7.0.0 | 程序集扫描 DI | Tier2 模块发现 | 已落地（ADR-115） | `ModuleBootstrapper` 扫描 `ICapabilityModule` |
| **Polly** 8.7.0 | 弹性管线 | `ResilienceCatalog` | 已落地（ADR-115） | default 管线：重试(指数退避)+熔断+超时 |
| **Carter** 10.0.0 | 模块化 HTTP | `ICarterModule` | 已落地（ADR-115） | `AddCarter()` + `MapCarter()` |
| **HybridCache** 10.9.0 | 设备影子缓存 | `cache.hybrid` 模块 | 已落地 | 10.9.0 预览版 `IHybridCache` 为非公共 API；改用 `HybridCache.GetOrCreateAsync/SetAsync/RemoveAsync`（`/sys/cache/shadow/{id}` 已验证命中/未命中） |
| **ScottPlot** 5.1.59 | 图表（HMI/看板） | `chart.scottplot` 模块 | 已落地（**改 SVG**） | `GetImageBytes`（PNG/SkiaSharp 栅格）在本环境**挂死**（实测 >40s）；改用 `GetSvgHtml(800,400)` 提取 `<svg>` 矢量，以 `image/svg+xml` 返回（`/sys/chart/telemetry` 已验证 200/31751B） |
| **OpenTelemetry** 1.17.0（+ AspNetCore + OTLP） | 可观测性 | `obs.otel` 模块 | 已落地 | OTLP→`localhost:4317`，Aspire 仪表盘就绪（`/sys/observability`） |
| **CommunityToolkit.Diagnostics** | 防御性编程 | `diag.toolkit` 模块 | 已落地 | `Guard`/`ThrowHelper` 演示 |
| **Foundatio** | 缓存/队列抽象 | `foundatio` 模块 | 已落地（**评估**） | 与 HybridCache/Channel 重叠；仅演示 `ICacheClient` API，不进核心依赖 |
| **Dapr** 1.18.x（复用 net10 资产） | AIOT 事件总线（opt-in） | `dapr` 模块 | 已落地（**opt-in 默认关**） | 配置门控 `Capabilities:Dapr:Enabled`（`/sys/dapr/status` 已验证） |
| **Native AOT** | 编译期硬化 | — | **Spike：暂缓（Blocked）** | 见 §2 |

## 2. Native AOT 硬化 Spike 结论

- **命令**：`dotnet publish -c Release -r win-x64 -p:PublishAot=true`（plc-host）
- **结果**：**失败（EXIT=1）**，约 60+ 编译错误，全部位于 ASP.NET Core **RequestDelegateGenerator（RDG）** 生成的 `obj/.../GeneratedRouteBuilderExtensions.g.cs`。
- **根因（直接）**：本宿主 minimal-API / Carter 端点大量返回**匿名类型**（`Results.Ok(new { ... })`）。RDG 无法为匿名返回类型生成 AOT 友好的序列化代码——先发 `RDG004`（Unable to resolve anonymous return type），继而生成**语法错误的 C#**（如 `anonymous` 标识符、`IEnumerable<T>` 缺类型参、未声明变量 `Id/Name/Version` 等），导致编译失败。
- **根因（结构性）**：宿主本质是"动态"的——`PluginManager` 经反射发现/构造 `IPlugin`（`GetTypes()` + `Activator.CreateInstance`）、Scrutor 程序集扫描、Carter 动态路由发现——均依赖反射，AOT 全程序修剪会裁掉这些路径，需 `[DynamicDependency]` / `DynamicallyAccessedMembers` + 修剪描述符标注。
- **可行性判定**：**本环境（.NET 11 Preview 7）暂时不可行 / 暂缓**。宿主的"插件化 + 扫描 + 动态路由"与 Native AOT 的静态全程序约束根本冲突。
- **若未来需启用 AOT，必需改造清单**：
  1. 将所有匿名类型端点返回改为**显式命名 DTO（`record`/`class`）**，使 RDG 经源生成 JSON 序列化（消除 `RDG004` 与生成错误）——涉及 `Modules/*` 全部返回体。
  2. `PluginManager` 入口标注 `[DynamicDependency(typeof(IPlugin), ...)]` 及对插件程序集的 `[DynamicDependency]`；对 `Activator.CreateInstance` 目标类型加 `[DynamicallyAccessedMembers]`。
  3. Scrutor 扫描入口加 `[DynamicDependency]` 覆盖被扫描程序集；Carter 路由发现同理（确认 Carter 10 是否提供 AOT 兼容源生成）。
  4. 评估 `<EnableRequestDelegateGenerator>` 与 Carter 的 AOT 兼容开关。
- **建议**：Native AOT **不在 plc-host 主线落地**；协议插件保持 ALC/JIT（ADR-118 §3）。若确有"单文件/极速启动"诉求，另立一个剔除动态性的 trimmed 核心，与插件宿主解耦。

## 3. 完成判据（优先级 2 收口）

- 9 项基础能力（含 opt-in Dapr）全部以 Tier2 模块形态落地，端到端验证通过：
  - `/sys/capabilities` 列出 7 模块（sys.info / cache.hybrid / chart.scottplot / obs.otel / diag.toolkit / foundatio / dapr）；
  - HybridCache 命中/未命中、OTel、Foundatio 缓存、Dapr status、ScottPlot SVG 均实测通过。
- Native AOT 结论明确：暂缓，附改造清单。
- **优先级 2 收口完成**，下一步进入优先级 3（PLC 插件化，ADR-118）。
