# 未完成项完成度评估（2026-08-25）

> 依据真实代码/grep/ADR 文档逐项核验。图例：✅ 已完成 · 🔶 部分完成/待验证 · ❌ 未完成
> 范围：appsettings/Dockerfile、前端 app.js、社媒/Notion 接入、本地 PG+EMQX、plc-saas.csproj、ADR-113 桥、POC、集成报告、P2 能力栈(ADR-117)。

## 总览

| 项 | 状态 | 一句话结论 |
|----|------|-----------|
| 1. 添加 appsettings + Dockerfile 容器化 | 🔶 | appsettings 齐；仅 WebApi 有 Dockerfile，SaaS/host 缺独立 Dockerfile |
| 2. app.js 改为消费 /api/... 并按页取数 | ❌ | 根 SPA 仍吃 `data/data.json` + HTMX，无 /api、无分页 |
| 3. 社媒/Notion 并入 + 统一导航/主题 + 移除落地页 | 🔶 | 已并入统一导航/主题（仍 mock）；落地页样例未接线、文件仍在 |
| 4. 验证并同步文档 §17 本地 PG+EMQX 容器 | 🔶 | 文档+脚本齐；沙箱无法实跑 wslc/EMQX，端到端实测未做 |
| 5. 补全 plc-saas 根 .csproj | 🔶 | 结构已补全；strip 目标被注释且源含 `#include`，构建未验证 |
| 6. ADR-113 真实 MQTT/EMQX 摄取桥 还原+构建验证 | ✅(实现)/⚠(构建验证挂 #5) | 代码+ADR+清单 P0 ✅；构建验证与 #5 同源 |
| 7. 最小 POC（生产者-消费者） | 🔶 | 模式已在 Pipeline/BusProducer 真实落地；独立最小 POC 未单独成件 |
| 8. 集成评估与结论报告 | ✅(存在)/待刷新 | PLC-IoT-Architecture-Verification.md 已含结论；日期 08-16、不含新层 |
| 9. 接入 ScottPlot 图表能力 | ✅ | ScottPlotCapability.cs（SVG 矢量，/sys/chart/telemetry 已验证） |
| 10. 接入 OpenTelemetry + Aspire 可观测性 | ✅ | ObservabilityCapability.cs（OTLP，Aspire 就绪） |
| 11. 接入 CommunityToolkit (Diagnostics) | ✅ | CommunityToolkitCapability.cs（Guard/ThrowHelper） |
| 12. Foundatio 评估与能力模块接入 | ✅ | FoundatioCapability.cs（评估，仅演示 ICacheClient，不进核心依赖） |
| 13. Dapr opt-in 能力模块接入 | ✅ | DaprCapability.cs（门控 `Capabilities:Dapr:Enabled`，默认关） |
| 14. Native AOT 编译期硬化 Spike | ✅ | Spike 完成，结论=暂缓/Blocked（ADR-117 §2，含改造清单） |
| 15. 汇总优先级2能力栈并补 ADR-117 | ✅ | docs/decisions/ADR-117-p2-capability-stack-status.md（Accepted） |

**计数：已完成 9 · 部分/待验证 5 · 未完成 1**（不含 #6/#8 的子项旗标）。

---

## 逐项证据与建议

### 1. 添加 appsettings + Dockerfile 容器化 — 🔶
- ✅ `src/plc-saas/appsettings.json`、`src/PlcAiot.Web/appsettings.json` 均已存在。
- ⚠ 仅根 `Dockerfile` 存在，面向 **PlcAiot.WebApi**（.NET 10 多阶段，完整可用）。
- ❌ `plc-saas` / `plc-host` 无独立 Dockerfile；`plc-saas/docker-compose.yml` 仅编排 PG+EMQX 基础设施，未把应用服务容器化。
- **建议**：若需 SaaS/host 镜像，补 `plc-saas/Dockerfile` 与 `plc-host/Dockerfile`（注意 #5 的 `#include`/strip 问题需先解决才能构建）。

### 2. app.js 改为消费 /api/... 并按页取数 — ❌
- 证据：`wwwroot/js/app.js` 仍 `axios.get("data/data.json")` + `htmx.ajax(...)` 片段切换；`loadData()` 一次性全量拉取，无分页、无 `/api/...` 调用；数据来自 mock `wwwroot/data/data.json`。
- 注：此前切真实后端的是 `src/PlcAiot.Web/wwwroot/pages/{cockpit,devices}.html`（同源 `/api/gateway/*`），属另一套静态页，与本根 `app.js` SPA 无关。
- **建议**：将 `loadData()` 改为按页/按资源的 `fetch('/api/...')`，并引入分页参数（offset/limit）；social/notion/account/ops 等路由同步切换数据源。

### 3. 社媒追踪与 Notion 仪表板并入、统一导航/主题、移除落地页 — 🔶
- ✅ `app.js` 的 `GROUPS` 已含 `social`（社媒矩阵）+ `notion`（团队协作），`renderers.social/notion` 存在；`#themeBtn` 主题切换与 `buildNav()` 统一导航已就位 → 并入 + 统一导航/主题完成（仍渲染 mock）。
- ⚠ `src/PlcAiot.Web/landing.html` 为独立样例，代码无引用（grep 零命中），未接入运行 SPA；文件仍在磁盘。可视作"未接线即等效移除"，但样例文件未删。
- **建议**：删除/归档 `landing.html` 样例（若确认不再需要）；将 social/notion 渲染切到真实后端（同 #2）。

### 4. 验证并同步文档 §17 启动本地 PostgreSQL + EMQX 容器 — 🔶
- ✅ `docs/E2E-dev-checklist.md` §1「起本地基础设施(wslc)」+ `src/plc-saas/start-dev-env.ps1`（已加固，提交 `6c5ccd8`）+ `plc-saas/docker-compose.yml`（PG18 / EMQX 6.1.4）。
- ⚠ 实际运行需 `wslc` + EMQX；沙箱无法探测 5432/EMQX（`Phase4-Verification.md` 标注运行期冒烟留作部署前置）→ 文档+脚本齐，端到端实测未做。
- **建议**：在具备 wslc 的环境跑一次 `start-dev-env.ps1` + `dotnet run`，回填实测结果到清单。

### 5. 补全 plc-saas 根 .csproj — 🔶（结构已补全，构建未验证 + 风险）
- ✅ `src/plc-saas/plc-saas.csproj` 已列出全部切片 + 包引用 + Content（appsettings/sql/docker-compose/initdb），结构完整。
- ⚠ strip 目标被整体注释（78–87 行），而源文件 `entities.cs / infra.cs / Program.cs / seed.cs` 含 file-based `#include` 指令；直接 `dotnet build` 可能因 `#include` 非标准 C# 而失败。
- **建议**：恢复 `PrepareStrippedApp` 目标（或改为预处理后编译），并实跑 `dotnet build src/plc-saas` 确认 0 错误。

### 6. ADR-113 真实 MQTT/EMQX 摄取桥 还原+构建验证 — ✅(实现) / ⚠(构建验证挂 #5)
- ✅ `docs/ADR-113-implementation.md`「V1-now 已落地」；代码 `src/plc-saas/slices/ingest.bridge.cs` + `ingest.http.cs`；`E2E-dev-checklist` P0 ✅（HTTP 主路径 + MQTTnet 边缘中继双路径）。
- ⚠ 构建验证与 #5 同源（plc-saas 构建）——待 #5 构建确认后一并验证。

### 7. 最小 POC（生产者-消费者） — 🔶
- ✅ 生产者-消费者模式已在多处真实落地：`src/plc-gateway/Pipeline.cs`（Disruptor 管线）、`src/plc-vsa-demo/plc-vsa-demo.cs`（`BusProducer` + 3 消费者）、`src/csgo-spike/src/GoTest/Program.cs`（Producer/Consumer 测试）。
- ⚠ 独立的"最小 POC"交付物（如 `ProducerConsumer.cs` spike）未明确存在。
- **建议**：若需独立极简样例，从 `Pipeline.cs` 抽一个最小 Channel/Disruptor 示例即可。

### 8. 集成评估与结论报告 — ✅（存在）/ 待刷新
- ✅ `docs/PLC-IoT-Architecture-Verification.md`（V9 实现核验报告，含"一句话结论"+落地占比+关键缺口）；另有 `Phase4-Verification.md`。
- ⚠ 报告日期 2026-08-16，明确不含 `plc-saas/` SaaS 新层与后续 `plc-host`/cockpit 工作。
- **建议**：刷新报告覆盖 ADR-113/ADR-117、plc-host 两层模块化、cockpit 真实后端接入等新增内容。

### 9–15. P2 能力栈（依据 ADR-117，全部 ✅）
统一以 `ICapabilityModule` + 可选 `ICarterModule` 形态落地，Scrutor 扫描，`/sys/capabilities` 可列：
- **9 ScottPlot**：`ScottPlotCapability.cs`，PNG 栅格在本环境挂死，改用 `GetSvgHtml` 提取 `<svg>` 矢量，`/sys/chart/telemetry` 已验证 200。
- **10 OpenTelemetry + Aspire**：`ObservabilityCapability.cs`，OTLP→`localhost:4317`，Aspire 仪表盘就绪（服务侧接入，无需 Aspire.Hosting）。
- **11 CommunityToolkit.Diagnostics**：`CommunityToolkitCapability.cs`，`Guard`/`ThrowHelper` 演示。
- **12 Foundatio**：`FoundatioCapability.cs`，评估态，仅演示 `ICacheClient`，与 HybridCache/Channel 重叠，不进核心依赖。
- **13 Dapr**：`DaprCapability.cs`，opt-in，`Capabilities:Dapr:Enabled` 门控，默认关，`/sys/dapr/status` 已验证。
- **14 Native AOT Spike**：`dotnet publish -r win-x64 -p:PublishAot=true` 失败（60+ RDG 错误，匿名类型端点 + 反射扫描/插件与 AOT 冲突），结论=暂缓，附改造清单（ADR-117 §2）。Spike 交付完成，AOT 本身不主线落地。
- **15 ADR-117**：`docs/decisions/ADR-117-p2-capability-stack-status.md`（Accepted），收口 P2 全部能力 + Native AOT 结论。

---

## 优先级建议（下一步）
1. **#5 构建验证**（恢复 strip 目标 + `dotnet build src/plc-saas`）—— 解锁 #6 构建验证，并消除潜伏编译阻断。
2. **#2 / #3** 前端接真实后端（根 SPA 的 `/api` 化 + 分页），与已完成的 cockpit/devices 同源方案对齐。
3. **#1** 补 `plc-saas`/`plc-host` Dockerfile（依赖 #5 先可构建）。
4. **#8** 刷新集成核验报告，覆盖新层。
