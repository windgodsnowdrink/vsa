# SaaS 能力补全推荐方案（Phase 5 立项建议）

> 版本：Rec v1.0 · 状态：**已被 ADR-114（2026-08-18 用户裁定）战略重排覆盖** —— 原“P0/P1 收口进入 P2”作废；新主轴 = **插件体系优先（DotNetCorePlugins，优先级 1）→ 基础能力栈（优先级 2）→ PLC 插件化（优先级 3）**；多租户 SaaS 重要等级降低（凭据默认 root/admin）。ADR-109 由 Deferred 反转升为优先级 1。
> 范围：把 `PLC-IoT-Architecture-Complete.md`(V9) 中「仅文档、未落地」的 5 项能力给出实现推荐，并补齐 DevOps / 安全审查 / 代码 CoreReview 三套工程治理。
> 关联：`docs/PLC-IoT-Architecture-Verification.md`（实现核验，落地≈40%）· `docs/SaaS-Architecture-Spec.md` · `docs/decisions/ADR-109-113.md`（PROPOSED）。

---

## 0. 总览：优先级与分期

| # | 能力 | 现状 | 优先级 | 推荐分期 | 是否核心数据平面 |
|---|------|------|--------|----------|------------------|
| 5 | 真实 MQTT/EMQX 客户端 | 空壳 `MQTT.cs`；ADR-107 已定隔离但无桥 | **P0** | V1-now（紧随 SaaS 收口） | ✅ 是（关键路径） |
| 1 | PluginManager + ALC 热插拔 | 根驱动空壳；真实驱动在 `samples/` | P1 | Phase 5（边缘域） | 边缘值，非控制面 |
| 4 | MEAI + MCP + Qdrant | `AI.cs` 空壳 | P1 | Phase 5（差异化） | 否 |
| 2 | CQRS Mediator + Wolverine Outbox | Mediator 已实现；自建 Outbox 已通过 | P2 | Later（按需增强） | 否 |
| 3 | YARP / WAF | 无 | P2 | Later（公网时） | 否（入口） |

**治理三件套（本次直接落地）**：DevOps 流水线、安全审查、代码 CoreReview —— 均 P0 工程基线，与 V1-now 绑定。

---

## 1. 真实 MQTT/EMQX 客户端 ⭐ P0

**现状（核验结论）**：`MQTT.cs` 为 File-based App 空壳（仅空构造函数）；`ADR-107` 已规定「租户级 MQTT 凭据 + EMQX JWT auth + HTTP AuthZ ACL，九段主题不变（因 `DeviceId` 全局唯一）」，但**没有任何桥接代码**。`plc-saas` 的 `TelemetryIngested` 事件无订阅者，设备遥测/故障进不来。

**推荐方案（ADR-113）**：
- **主路径：EMQX 规则引擎 → `plc-saas` HTTPS 摄取端点**（无状态、横向易扩展）。EMQX 收到设备发布后，按规则转发到 `POST /api/v1/ingest/telemetry` 与 `/ingest/fault`。
- **边缘中继（可选）：`MQTTnet` 订阅租户主题**，用于边缘聚合后再上行。
- **摄取流程**：中间件校验 `tid`（JWT/ACL 已保证）→ 映射 `Device`（全局唯一）→ 发 `TelemetryIngested`/`FaultDetected` 经 Mediator → Outbox → 写 **InfluxDB（遥测时序）+ PostgreSQL（故障）** → **SignalR `/hubs/faults` 推送**（ADR-105 强一致）。
- **隔离**：沿用 ADR-107 的 EMQX JWT（`tid` 声明）+ ACL；控制面侧 `TenantMiddleware` 注入 `ICurrentTenant`，EF 全局过滤器 + RLS 兜底。
- **依赖**：`MQTTnet`（边缘中继）；摄取端点无需新依赖。

**风险**：EMQX 部署拓扑未定（内网自建 vs 云）。**建议**：V1 内网先用 EMQX 开源版单节点 + 规则引擎转发，摄取端点随 `plc-saas` 部署。

---

## 2. PluginManager + ALC 热插拔（P1，边缘域）

**现状**：根目录 `.cs` 协议驱动是 File-based 空壳；真实驱动在 `samples/PlcAiot.Stability` 实现 `IDeviceProtocol`/`DeviceCatalog`/`DeviceCapabilities`，但**未接回根驱动层**，与 V9 §1.12 统一契约脱节。

**推荐方案（ADR-109）**：
- **驱动收敛**：把所有协议驱动统一到 `IDeviceProtocol`，消除「根空壳 vs samples 真驱动」双轨。这是热插拔的前提。
- **PluginManager（ALC）**：用 `AssemblyLoadContext`（DotNetCorePlugins）在**边缘 JIT 宿主**（`samples/.../file-based/plugin-host.cs`）加载 `plugins/` 目录驱动，支持文件变更后卸载/热重载。
- **AOT 约束**：`edge-aot-host.cs`（NativeAOT）**不能**用 ALC（AOT 需静态链接）。故 AOT 边缘宿主发固定驱动集；热插拔仅 JIT 边缘宿主支持。文档须明示此权衡。
- **控制面不涉及**：`plc-saas` 是 SaaS 控制面，消费遥测即可，不需要运行时插件加载。PluginManager 留在边缘/稳定性包。

**风险**：驱动收敛工作量不小；ALC 与 NativeAOT 不兼容需在文档与构建矩阵中固化。

---

## 3. CQRS Mediator + Wolverine Outbox（P2，增强）

**现状**：`plc-saas` **已实现** CQRS 派发（Mediator MIT 源码生成 `[EventHandler]`）+ **自建事务 Outbox 表 + 轮询 Agent**（ADR-102），`dotnet test` 2/2 通过。

**推荐方案（ADR-110）**：
- **MVP 保留自建 Outbox**：已通过、零 preview 期外部依赖，不回退。
- **Wolverine 采用时机**：当接通真实 EMQX 传输（#1/#5）或需要成熟 inbox/调度消息时，引入 Wolverine 的 **EF Core 事务 Outbox**（与 `SaveChanges` 同事务自动派发，替代手搓轮询 Agent）。CQRS 派发仍用 Mediator，不替换。
- **判定**：属延迟收益，非阻塞项，列入 Later。

---

## 4. YARP / WAF（P2，入口）

**现状**：仓库无任何网关/WAF 代码。SaaS 为内网免费使用（ADR-108 裁定）。

**推荐方案（ADR-111）**：
- **V1 内网**：轻量 **YARP** 前置容器，职责：路径路由（`/api/v1/*`→`plc-saas`，`/`→`wwwroot` 静态）、TLS 终止、全局限流中间件（喂给配额 429 故事）。
- **WAF**：内网靠网络隔离 + YARP 请求大小/头限制即可；**ModSecurity 类 WAF 推迟至公网暴露**。
- **替代**：若部署前已有 nginx/traefik 入口，可省略 YARP，改在入口配置。需先定部署拓扑。

---

## 5. MEAI + MCP + Qdrant（P1，差异化，Phase 5）

**现状**：`AI.cs` 空壳。

**推荐方案（ADR-112）**：
- **MEAI**（`Microsoft.Extensions.AI`）作 LLM 抽象，兼容 OpenAI/Azure/OLLama。
- **MCP Server**：把设备/故障/遥测暴露为 MCP 工具，**`tid` 作用域隔离**（工具调用自动注入当前租户，禁止跨租户查询）——战略契合「AIOT」品牌，外部 AI Agent 可安全问诊。
- **Qdrant**：存故障模式向量（租户分区 collection），支撑 RAG / 异常检索。
- **门控**：`AiAssistant` feature flag；默认关，Phase 5 启用。

---

## 6. 工程治理（本次直接落地，P0 基线）

| 治理件 | 交付物 | 说明 |
|--------|--------|------|
| DevOps 流水线 | `.github/workflows/ci.yml` + `docs/DevOps-Pipeline.md` | 构建/测试/安全/审查四关卡；.NET 11 preview SDK 钉选 |
| 安全审查 | `docs/Security-Review.md` | 密钥扫描 + 依赖 CVE + SAST(Semgrep) + 多租户隔离审查清单 + OWASP 映射 |
| 代码 CoreReview | `docs/Code-Review.md` + PR 模板 + `CODEOWNERS` + `scripts/review-gates.sh` | P0 自动门禁（emoji/硬编码色/货币红线/紫粉渐变）复用既有 QA 扫描 |

---

## 7. 待用户确认的分期决策

- **V1-now（建议立即立项）**：#5 真实 MQTT/EMQX 桥（否则 SaaS 无数据）+ 治理三件套。
- **Phase 5（下一迭代）**：#1 驱动收敛+ALC、#4 MEAI/MCP/Qdrant。
- **Later（按需）**：#2 Wolverine Outbox、#3 YARP/WAF（公网时）。

> 5 项 ADR（109~113）以 PROPOSED 写入 `docs/decisions/ADR-109-113.md`，待架构师（高见远）ratify 后转正。
