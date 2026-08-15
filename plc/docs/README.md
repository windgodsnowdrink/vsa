# PLC·AIOT 文档索引（README）

> 本目录文档分两大层：**A 层 = 旧单租户 PLC/IoT 平台**（既有，V9 架构 + 蓝图 + 演示）；**B 层 = 新 SaaS 多租户**（Phase 1–3 已构建并通过门禁，增量升级）。
> 读者先判自己属于哪一层，再读对应入口，避免把"蓝图/演示"误当"已上线实现"。
> 实现核验详见 [`PLC-IoT-Architecture-Verification.md`](./PLC-IoT-Architecture-Verification.md)。

---

## 如何使用本索引

- 想了解**整体平台（单租户）设计蓝图** → 读 A 层入口
- 想了解**多租户 SaaS 升级（租户/身份/计费/配额）** → 读 B 层入口
- 想看**架构图** → `saas-architecture.svg`（SaaS 控制面）或 `architecture-merged.svg`（旧全平台大图）
- 想看**某声明是否真的落地** → 查 `PLC-IoT-Architecture-Verification.md` 的逐项核验表

---

## A 层 · 旧单租户 PLC/IoT 平台（既有）

> 基线：.NET（仓库实际 net11.0 Preview，文档标称 .NET 10）。状态多为「设计蓝图 / 演示骨架」，非生产全栈。

### A-1 架构与设计（核心阅读）

| 文档 | 状态 | 说明 / 读者入口 |
|------|------|----------------|
| [`PLC-IoT-Architecture-Complete.md`](./PLC-IoT-Architecture-Complete.md) | 设计蓝图 V9（部分实现） | **旧架构总文档**。模块化单体 + ALC 热插拔 + 三平面 + 长稳/互换/故障三大支柱。**注意：多处声明仅为蓝图，见核验报告** |
| [`PLC-IoT-HotPlug-Architecture.md`](./PLC-IoT-HotPlug-Architecture.md) | 调研/设计 | 底座/能力/入口/出口/框架推荐（被 Complete 合并前的基础） |
| [`PLC-IoT-Design-Decisions.md`](./PLC-IoT-Design-Decisions.md) | 调研/设计 | 具体选型决策与代码骨架 |
| [`PLC-IoT-Architecture-Review.md`](./PLC-IoT-Architecture-Review.md) | 评审 | V7 评审加固（三条铁律、G 系列质量目标） |

### A-2 架构图（SVG）

| 文件 | 覆盖 | 与 SaaS 关系 |
|------|------|--------------|
| [`architecture-merged.svg`](./architecture-merged.svg) | 旧全平台大图（YARP/WAF/InfluxDB/Seq/ClickHouse 等） | 不含多租户层 |
| [`architecture-diagram.svg`](./architecture-diagram.svg) | 旧插件隔离区 / ALC / 适配器 | 不含多租户层 |
| [`data-plane-diagram.svg`](./data-plane-diagram.svg) | 旧协议插件 / EMQX / Outbox / PostgreSQL 数据平面 | 不含多租户层 |

### A-3 代码落地位置（不在 docs/，供对照）

- `samples/PlcAiot.Stability/` —— **真实可运行骨架**：`IDeviceProtocol`/`DeviceCatalog`（统一契约 + 热替换）、`DeviceSessionSupervisor`（看门狗自愈）、`FaultEvent`/`IFaultStore`/`IRecoveryStrategy`/`ResolutionStateMachine` + `PostgreSqlFaultStore`(Npgsql) / `InfluxDbFaultStore`（故障全生命周期，PG/InfluxDB 双写）、`file-based/`（edge-aot-host / plugin-host 两个 File-based App 样例）。
- `samples/PlcAiot.Web/` —— 前端 mockup（landing / cockpit 8页SPA+Three.js / social-matrix-tracker / notion-dashboard）。
- 仓库根 `*.cs`（Modbus/MQTT/AI 等）—— **多为 File-based App 空壳占位**；真实驱动在 Melsec/LoRa/IoT/Panasonic/Schneider/Keyence/Fatek/Fuji（走 `DriverBase`，未接 `IDeviceProtocol`）。

---

## B 层 · 新 SaaS 多租户（Phase 1–3 已构建 / 已验证）

> 增量升级：在既有 PLC/IoT 平台之上叠加多租户控制面（共享 PostgreSQL + TenantId 行级隔离 + RLS）。Billing 仅计量 + 配额强制，**不计费、不收费**（ADR-108）。

### B-1 规格即契约（核心阅读，按顺序）

| 文档 | 状态 | 说明 / 读者入口 |
|------|------|----------------|
| [`SaaS-PRD.md`](../SaaS-PRD.md) | 已确认 | 多租户 SaaS 增量 PRD（用户故事 / RICE / 范围） |
| [`SaaS-Architecture-Spec.md`](./SaaS-Architecture-Spec.md) | **v1.1 · Phase 3 已构建并通过门禁** | SaaS 架构规格（选型矩阵 / 隔离实现 / ADR-100~108 / 切片划分 / 启动引导）。**最新总入口** |
| [`SaaS-Spec.md`](./SaaS-Spec.md) | 已生成 | 12 章规格契约（范围/API/DB/页面/Token/验收/坑） |
| [`openapi.yaml`](./openapi.yaml) | 已验证 | 21 端点 OpenAPI 3.2（bearerAuth + tid 语义 + 403 拒绝规则） |
| [`SaaS-DB-Schema.md`](./SaaS-DB-Schema.md) | 已验证 | 10 表 + `ITenantEntity` + EF 全局过滤器 + PostgreSQL RLS |
| [`SaaS-Metering.md`](./SaaS-Metering.md) | 已验证 | Outbox 轮询 → 聚合 → 配额评测（Warn/Throttle）→ SignalR 推送 |
| [`SaaS-Page-Prompts.md`](./SaaS-Page-Prompts.md) | 已验证 | 9 页 SaaS 提示词 + 租户切换器（前端开发依据） |

### B-2 设计系统

| 文件 | 状态 | 说明 |
|------|------|------|
| [`design-tokens.json`](./design-tokens.json) | 已验证 | SaaS 扩展 `color.saas.*` + 图标/徽章/阈值 Token |
| [`design-tokens.css`](./design-tokens.css) | 已验证 | Token 落地（图标类 / 套餐档徽章 / 租户点 / 作用域条） |

### B-3 架构图（SVG）

| 文件 | 覆盖 | 说明 |
|------|------|------|
| [`saas-architecture.svg`](./saas-architecture.svg) | **SaaS 多租户控制面（新建）** | C4 容器级：客户端 / plc-saas(21 端点+SignalR+Mediator+Outbox) / PostgreSQL+RLS / InfluxDB / EMQX / 隔离纵深 + ADR 徽标 |

### B-4 架构决策（ADR）

| 文档 | 状态 | 说明 |
|------|------|------|
| [`decisions/ADR-100-107.md`](./decisions/ADR-100-107.md) | 已落地 | ADR-100~107（隔离/选型/切片/SignalR/MQTT 方案 B 等） |
| `decisions/ADR-100-107.md` 增 **ADR-108** | 已补 | Billing MVP = 计量 + 配额强制，**不计费**（内网免费） |

### B-5 代码落地位置

- `plc-saas/`（分支 dev）—— File-based App：Program.cs + infra.cs + slices/（21 端点 + SignalR Hub）+ entities.cs + sql/rls.sql（启动自动建表 + 应用 RLS）。已提交 `0cd0f31`(切片) / `594ebe9`(引导) / `613c620`(中间)。
- `wwwroot/`（分支 dev）—— 9 页 SaaS + 租户切换器 + design-tokens.css（前端 `df668f0`）。

---

## 状态图例

- ✅ **已实现/已验证**：代码落地且通过门禁
- 🟡 **部分实现/演示**：samples 骨架或 mockup，非生产全栈
- 📐 **设计蓝图/仅文档**：描述详尽但仓库内无对应实现（如 PluginManager+ALC、CQRS+Wolverine、YARP、MEAI+MCP+Qdrant、真实 MQTT/EMQX 客户端）
- 📝 **调研/评审**：过程性文档

> 旧架构的"仅文档"项见 [`PLC-IoT-Architecture-Verification.md`](./PLC-IoT-Architecture-Verification.md)，落地占比约 40%。
