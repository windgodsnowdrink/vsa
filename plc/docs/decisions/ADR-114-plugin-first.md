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

## Open Questions（开放问题 · 待用户拍板）

1. **Oqtane 范围**：Oqtane 是 Blazor 模块化 CMS。是指“采用 Oqtane 作为模块/插件宿主”，还是仅取其“模块化应用框架”思路、自研轻量宿主？两者工作量与耦合度差异极大。
2. **“CSGO”指代**：疑似 typo。候选 —— Ocelot（API 网关）/ Kafka（流）/ Orleans·Akka（Actor）/ 其他？需明确。
3. **Dapr vs Flink**：Dapr = 微服务 sidecar 构建块；Flink = 流处理。二者范式不同，是否都需要、还是只取其一作为编排/流底座？
4. **插件宿主落点**：插件体系内嵌进 `plc-saas` 控制面，还是新建独立“插件宿主 / 边缘宿主”工程作为主产物？决定代码组织与 ALC 隔离边界。
5. **基础能力深度**：统一“轻量引用 + flag 门控 + 离线兜底”（如 ADR-112 的 AI 做法），还是对部分组件做“深集成”？影响风险与工期。

---

## Related

- ADR-109（本 ADR 反转其 Decision ④）、ADR-110（Deferred→P2）、ADR-111（Deferred→P2）、ADR-112（降为差异化能力）、ADR-113（P0 已落地，保留）。
- `docs/PLC-IoT-Architecture-Complete.md` §0.4.1：上述组件在原文档中多标为【未采纳】/【边缘域】，本 ADR 将其重排为【优先级 2 地基】，待开放问题收敛后更新落地状态。
