# ADR-114：插件优先 + 基础能力栈（战略重排）

- **Status**：Accepted（2026-08-18，用户裁定）
- **格式**：MADR · 关联：ADR-109（反转）、`docs/SaaS-Phase5-Recommendations.md`（已被本 ADR 覆盖）、`docs/PLC-IoT-Architecture-Complete.md` §0.4.1（技术落地状态对照）
- **技术基线**：.NET 11 Preview + VSA + DDD + CQRS + File-based App

---

## Background（背景）

原路线图（`SaaS-Phase5-Recommendations.md`，2026-08-16 拍板）将：
- **多租户 SaaS 控制面** 作为主轴（P0/P1 已收口）；
- **DotNetCorePlugins 插件体系（ADR-109）** 判为“边缘域 Deferred，控制面不涉及运行时插件加载”（Decision ④）；
- Oqtane / Foundatio / Polly / Scrutor / Carter / ScottPlot / HybridCache / OTel+Aspire / CommunityToolkit / Dapr·CSGO·Flink / Native AOT 等在 §0.4.1 落地状态对照中被标为【未采纳】或【边缘域】。

2026-08-18 用户裁定**战略方向反转**：
1. 插件体系升为**最高优先级**，优先于多租户 SaaS 精修；
2. 在插件体系 + 基础能力之上，再实现 **PLC 插件化**；
3. 多租户 SaaS 重要等级**降低**，凭据先默认 `root` / `admin`。

---

## Decision（决策）

1. **DotNetCorePlugins 插件体系 = 优先级 1（先行）**：反转 ADR-109 Decision ④ 旧裁定；`plc-saas` 控制面或独立插件宿主需具备运行时加载 `plugins/` 目录程序集的能力（ALC 卸载/热重载）。
2. **基础能力栈 = 优先级 2（地基）**：在插件体系与 PLC 插件化之前，先把下列组件接入作为公共地基 ——
   Oqtane、Foundatio、Polly、Scrutor、Carter、ScottPlot、HybridCache、OTel+Aspire、CommunityToolkit、Dapr / CSGO / Flink、Native AOT。
   （各组件“轻量引用 vs 深集成”的尺度见开放问题。）
3. **PLC 插件化 = 优先级 3**：在①②之上，将设备协议驱动（`IDeviceProtocol` / `DeviceCatalog`）与业务模块以**插件形态**加载，消除根目录空壳与 `samples/` 真驱动的双轨。
4. **多租户 SaaS 降级**：不再作为主轴；种子凭据默认 `root` / `admin`（已在 `appsettings.json` + `seed.cs` 落地）；隔离/RLS/配额等已落地能力保留，精修工作后置。

---

## Consequences（后果）

- 正面：平台从“SaaS 优先”转向“**可插拔内核优先**”，更契合 PLC·AIOT 的协议/设备多样性与边缘部署诉求；基础能力栈统一后，插件与 SaaS 共享同一套弹性/缓存/观测/模块化底座。
- 负面：
  - 多租户 SaaS 精修（如租户自助开通、计费计量深化）推迟；
  - 12 项基础能力若“深集成”会显著扩大 preview 期依赖面与构建矩阵（尤其 Native AOT 与 ALC、OTel+Aspire 的兼容需验证）；
  - “CSGO”指代不明，Dapr 与 Flink 属不同范式，须先收敛。

---

## Decisions Resolved（已拍板 · 2026-08-18）

1. **Oqtane 范围**：**不引入 Oqtane 全栈**。仅取其“模块化”思路，自研基于 ALC 的轻量插件宿主（最小依赖、可控、契合 File-based App）。→ 见 `plc/plc-host/`。
2. **“CSGO”指代**：**CSGO = CsGo**（`github.com/HAM-2015/CsGo`）；与 **Dapr**（`github.com/dapr`）和 **CSharpFlink**（`github.com/wxzz/CSharpFlink`）**三者都要**，作为编排/流处理底座。
3. **Dapr vs Flink**：二者并存——Dapr 管微服务 sidecar 构建块，Flink（CSharpFlink）管设备遥测流处理；CsGo 作补充（网络/游戏式协议栈，视边缘场景）。
4. **插件宿主落点**：**新建独立工程 `plc/plc-host/`**（主产物），`plc-saas` 控制面降为其一个插件消费者；宿主与插件通过 `Plc.Plugins.Contracts` 共享契约、ALC 隔离。
5. **基础能力深度**：统一 **“轻量引用 + feature flag 门控 + 离线兜底”**（沿用 ADR-112 的 AI 做法），preview 期风险最低，按需再深集成。

> 优先级 1 骨架已落地：`plc-host`（宿主 + PluginManager + ALC 隔离加载器 + 热重载）+ `Plc.Plugins.Contracts`（共享契约）+ `plugins/DemoDevicePlugin`（演示 PLC 设备协议插件，验证插件化）。基础能力栈（Dapr/CSharpFlink/CsGo/Polly/Scrutor/Carter/ScottPlot/HybridCache/OTel+Aspire/CommunityToolkit/Native AOT）列入优先级 2，待逐项接入。

---

## Related

- ADR-109（本 ADR 反转其 Decision ④）、ADR-110（Deferred→P2）、ADR-111（Deferred→P2）、ADR-112（降为差异化能力）、ADR-113（P0 已落地，保留）。
- `docs/PLC-IoT-Architecture-Complete.md` §0.4.1：上述组件在原文档中多标为【未采纳】/【边缘域】，本 ADR 将其重排为【优先级 2 地基】，待开放问题收敛后更新落地状态。
