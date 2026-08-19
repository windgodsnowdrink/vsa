# ADR-115：两层模块化架构（插件 ALC + 基础能力模块）与模块模式

- **Status**：Accepted（2026-08-19，优先级 2 首项落地）
- **格式**：MADR · 关联：ADR-114（优先级 2 基础能力栈）、`plc/plc-host/`
- **技术基线**：.NET 11 Preview + VSA + File-based App + DotNetCorePlugins 风格 ALC

---

## Background（背景）

ADR-114 将「基础能力栈」列为优先级 2 的地基，首项明确为 **Scrutor（程序集扫描 DI）+ Polly（弹性）+ Carter（模块化 HTTP）**，并要求「之后再把其他全部基础能力模块化」。

落地前需回答一个架构分层问题：基础能力（Oqtane 思路、Foundatio、Polly、Scrutor、Carter、ScottPlot、HybridCache、OTel+Aspire、CommunityToolkit、Dapr/CSGO/Flink、Native AOT）与优先级 1 已建的 **ALC 插件（设备协议）** 是什么关系？能否共用一套模块化机制？

若把基础能力也塞进 ALC 插件，会引入不必要的隔离开销与类型身份管理；若全部内联进宿主启动代码，又会回到「散落最小 API / 硬编码装配」的反模式，违背「模块化」初衷。

---

## Decision（决策）

确立 **两层模块化架构**，明确区分「信任边界」：

### Tier 1 — 插件（ALC，第三方 / 设备协议）
- 机制：原生 `AssemblyLoadContext` + `AssemblyDependencyResolver`（DotNetCorePlugins 风格），运行时加载 `plugins/` 目录程序集，**可卸载 / 热重载**。
- 契约：`Plc.Plugins.Contracts.IPlugin`（无参构造 + `StartAsync` / `StopAsync`）。
- 适用：第三方设备协议驱动、需隔离 / 热更新的不可信代码。
- 代码：`Plc.Host.Plugins.PluginManager` + `PluginLoadContext`。

### Tier 2 — 基础能力模块（宿主信任、编译期内置）
- 机制：**Scrutor 程序集扫描**自动发现 `ICapabilityModule` 实现并注册为单例，逐个调用 `RegisterServices` 完成装配；HTTP 面由 **Carter**（`ICarterModule`）自动发现并 `MapCarter()` 路由；跨进程 / IO 调用由 **Polly**（`ResilienceCatalog` 命名 `ResiliencePipeline`）统一包裹弹性。
- 契约：`Plc.Host.Foundation.ICapabilityModule`（可选同时实现 `Carter.ICarterModule`）。
- 适用：平台 curated 的基础能力栈（见 ADR-114 优先级 2 清单），均为信任代码、编译进宿主、随宿主版本发布。
- 代码：`Plc.Host.Foundation.ModuleBootstrapper`（Scrutor 扫描）、`Plc.Host.Foundation.Resilience.ResilienceCatalog`（Polly）、`Plc.Host.Modules.*`（Carter 模块）。

### 模块模式（后续「其他全部基础能力的模块化」机械复用）
> 新增一项基础能力 = 在 `plc-host` 中新增**一个类**，实现 `ICapabilityModule`；若需 HTTP 端点，再 `: ICarterModule` 写 `AddRoutes`。无需改动 `Program.cs`、无需改动 `PluginManager`。Scrutor 自动纳入，Carter 自动路由，Polly 按需注入 `ResilienceCatalog`。

---

## Consequences（后果）

- 正面：
  - 两层信任边界清晰——不可信 / 需热更新的走 ALC 插件，信任内置能力走模块，互不污染；
  - 「基础能力模块化」有了可复制范式，ADR-114 优先级 2 其余 11 项（ScottPlot/HybridCache/OTel+Aspire/CommunityToolkit/Dapr/CSGO/Flink/Native AOT/Foundatio 等）接入成本降到「加一个类」；
  - 弹性策略从「各模块各写重试」收敛为 `ResilienceCatalog` 统一管线，可观测、可统一调参。
- 负面 / 注意：
  - 两层机制并存，新成员需理解「何时用 IPlugin、何时用 ICapabilityModule」；
  - Scrutor 扫描在启动期用反射实例化模块并调 `RegisterServices`，模块构造必须无参且轻量（禁止在构造中做重 IO）；
  - Carter 模块内路由处理器的服务注入依赖 Carter 的参数绑定（与最小 API 一致），复杂依赖建议从 `HttpContext.RequestServices` 显式解析。

---

## Validation（验证）

`plc-host` 端到端验证（`dotnet run`，http://localhost:5000）：

| 端点 | 验证点 |
|---|---|
| `GET /sys/capabilities` | Scrutor 已发现 `sys.info` 模块 + Polly 管线 `default` 已注册 |
| `GET /sys/resilience-demo` | Polly 默认管线（重试3 + 熔断 + 超时）生效：每 3 次调用中第 3 次成功（"第 3/6/9 次成功"） |
| `GET /sys/info` | Carter 模块化路由正常（含运行时 / 框架 / 分层说明） |
| `GET /plugins`、`GET /healthz` | 原内联端点已收口为 Carter `PluginsModule` |

> 注：本次运行 `GET /plugins` 返回 `[]`，因 `plugins/DemoDevicePlugin` 仅含源码、未放置已构建的程序集；ALC 加载机制本身在 ADR-114 优先级 1 提交（1f99605）中已验证。放入构建产物后即恢复设备协议插件加载。

---

## Related

- ADR-114（优先级 2 基础能力栈、两层架构在此细化）、ADR-109（ALC 插件体系）、ADR-112（轻量门控，基础能力接入沿用）。
- 实现：`plc/plc-host/Foundation/*`、`plc/plc-host/Modules/*`、`plc/plc-host/Program.cs`。
