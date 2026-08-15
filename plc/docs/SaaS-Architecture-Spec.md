# PLC·AIOT 多租户 SaaS — 架构规格与决策

> 版本：Arch-Spec v1.1 · 状态：Phase 3 已构建并通过门禁（后端 0cd0f31 + 594ebe9，前端 df668f0）
> 角色：首席架构师（高见远）
> 技术栈：.NET 11 Preview 6（SDK 11.0.100-preview.6，C# 14） + VSA + DDD 清洁架构 + CQRS + File-based App
> 关联文档：`docs/PLC-IoT-Architecture-Complete.md`（V9 既有架构）、`DESIGN.md` §10（设计系统）、`SaaS-PRD.md` §9.1（产品对齐）、`docs/saas-architecture.svg`（SaaS 架构图）
> 最后更新：2026-08-15（Phase 3 收口 + 架构图 docs/saas-architecture.svg）

---

## 0. 范围与已验证前提

- 已读既有资产：架构 V9（领域模型 Devices/Faults/Supervisor/Persistent/Abstractions、九段 MQTT、边缘 SQLite/Mosquitto/Datasync、§17 前端）、现有 `Program.cs`、samples（`edge-aot-host.cs` / `plugin-host.cs` 两个已验证 File-based App、五程序集领域代码）、`PRD.md`、`global.json`。
- 联网核验（WebSearch，2026-08-15）：
  - .NET 11 Preview 6 已发布，SDK `11.0.100-preview.6`；File-based Apps 在 .NET 11 新增 `#include` 多文件拆分与 `dotnet user-jwts` 支持；OpenAPI 默认 3.2；ASP.NET Core 新增基于 Fetch Metadata 的自动 CSRF 防护。
  - EF Core 全局查询过滤器 + `ITenantEntity` 标记接口 + `SaveChanges` 拦截器、JWT `tid` 声明 + 中间件注入租户，为官方推荐的多租户共享 DB 方案；PostgreSQL RLS 作为兜底。
- `global.json` 当前 `"version": "11.0.100"` + `latestFeature` + `allowPrerelease`，需显式钉到 `11.0.100-preview.6`（见 ADR-100）。
- 说明：本机未找到架构知识库参考文件（`references/architecture/*.md` 等）；下列结论基于 .NET 11 官方文档与领域工程实践。

---

## ① 技术选型对比矩阵（推荐项 / 理由 / 版本锚定）

### 1.1 VSA 端点组织
| 方案 | 说明 | 评分 | 结论 |
|------|------|------|------|
| **A. Minimal API 内联 + File-based `#include` 共享基础设施**（推荐） | 每切片一 .cs 文件，内联 `MapGroup` + Mediator Handler；`#include "infra.cs"` 提供 builder/app/auth/db | 5 | 与 File-based + VSA 天然契合，零程序集扫描 |
| B. Carter 模块化端点 | `ICarterModule` 按程序集注册 | 2 | 依赖程序集扫描，与 File-based 单文件模型冲突，放弃 |
| C. FastEndpoints | 独立 REPR 类 | 3 | 强但需 project 结构，File-based 下不如 A 直接 |

### 1.2 CQRS 进程内派发
| 方案 | 说明 | 评分 | 结论 |
|------|------|------|------|
| **A. Mediator (martinothamar, MIT) 源码生成**（推荐） | `[EventHandler]` 编译期注册、零反射、NativeAOT 友好、`ICommand/IQuery` + `IPipelineBehavior` | 5 | 架构 V9 已定，MIT 合规 |
| B. MediatR | v12+ 转商业许可 | 1 | 放弃（非 MIT） |
| C. 原生端点直接调用 | 端点直调服务 | 3 | CQRS 退化，仅切片内部兜底 |

### 1.3 多租户隔离策略
| 方案 | 说明 | 评分 | 结论 |
|------|------|------|------|
| **A. 共享 DB + TenantId + EF Core 全局过滤器 + PostgreSQL RLS**（推荐，= Phase 0） | 逻辑隔离、单迁移、运维最低 | 5 | 500→5K 租户最优 |
| B. Schema-per-tenant | 中隔离、N schema 迁移 | 2 | 受监管行业才需 |
| C. Database-per-tenant | 最强隔离、运维最重 | 1 | 仅 Premium 未来可选 |

### 1.4 读模型 / CQRS 存储
| 方案 | 说明 | 评分 | 结论 |
|------|------|------|------|
| **A. 同库命令/查询分离模型 + 可选读副本（MVP 先不分）**（推荐） | 写聚合 + 投影读表，同事务同步投影；读副本仅分析查询 | 5 | MVP 不做事件溯源，避免过度工程 |
| B. PostgreSQL 主从 + 物化视图 | 读扩展 | 3 | 延后 |
| C. EventStore 事件溯源 | 全量事件重放 | 1 | MVP 不可行 |

### 1.5 多租户认证
| 方案 | 说明 | 评分 | 结论 |
|------|------|------|------|
| **A. ASP.NET Core Identity + JWT Bearer（access 15min / refresh 7d）**（推荐） | `tid` 声明承载租户；`dotnet user-jwts` 支持 File-based | 5 | 官方栈，零额外依赖 |
| B. OpenIddict / Finbuckle.MultiTenant | 完整 IdP | 3 | 企业 SSO 列入 Later |
| C. 纯 JWT 自签 | 无用户库 | 2 | 不满足 SaaS |

### 1.6 图标库（P0 硬约束）
- **锁定 Lucide（ISC，24×24，2px 描边，currentColor）** 为唯一 SVG 图标源；全站禁 emoji（ADR-106）。前端用雪碧图 `wwwroot/css/icons.svg` 或 `@lucide/...` 包，与 DESIGN.md §10 / SaaS-PRD.md §9.1（SSOT=Lucide）对齐。

### 版本锚定
- SDK `11.0.100-preview.6`（C# 14），rollForward=latestPatch，allowPrerelease=true
- EF Core `11.0.0-preview.6` + `Npgsql.EntityFrameworkCore.PostgreSQL` `11.0.0-preview.6`
- Mediator（martinothamar）3.x（MIT，含 SourceGenerator）
- 认证：内建 `Microsoft.AspNetCore.Authentication.JwtBearer`；OpenAPI 3.2 默认
- 图标：Lucide（ISC）；前端雪碧图 `wwwroot/css/icons.svg` 或 `@lucide/...` 包

---

## ② SaaS 架构草案

### 2.1 限界上下文地图（C4 容器级）
新增 Tenant / Identity / Billing 三大 SaaS 上下文与既有 PLC 领域共存；跨上下文仅经领域事件/API 通信（延续 V9 铁律：禁止直连对方库）。

```
                    ┌──────── SaaS 控制面（多租户）─────────────────────────┐
  管理/运营 ──▶  [Tenant] 注册·订阅·套餐        [Identity] 用户·角色·JWT
                    │                                    │ (tid 声明注入)
                    └─────── 所有 PLC 领域上下文带 TenantId ───────┘
   ┌──────────────────────────────────────────────────────────────────┐
   │  Devices  Faults  Supervisor  Telemetry  Command  Rule  Diagnosis  │
   │  每个聚合根/实体实现 ITenantEntity，写时自动盖 TenantId             │
   └──────────────────────────────────────────────────────────────────┘
   Billing — 消费 Tenant 订阅事件 + 遥测用量事件，仅计量 + 配额强制（不计费，ADR-108）
```

### 2.2 VSA 切片划分（File-based 形态，实建 21 端点 / 7 组）
```
plc-saas/
  Program.cs            # 顶层语句入口（#include 拼装 GlobalUsings/infra/各切片/seed）
  infra.cs              # #include 共享：builder/app、AddMediator、AddDb、AddAuth、中间件、拦截器
  slices/
    tenant.register.cs  tenant.activate.cs        # 租户注册/激活/暂停/恢复（4）
    identity.login.cs   identity.members.cs  identity.mqtt.cs   # 登录/刷新/成员/角色/MQTT 凭据（5）
    billing.usage.cs    billing.plan.cs     billing.quota.cs   # 用量/套餐/配额（3，ADR-108 仅计量）
    devices.list.cs                                # 设备列表（1）
    faults.ack.cs      faults.stream.cs            # 告警确认/实时流（2，ADR-105 SignalR）
    ops.tenants.cs     ops.pricing.cs  ops.roles.cs  ops.health.cs  # 运营（4）
    metering.agent.cs                             # 后台 HostedService 轮询 Outbox 聚合
```
每切片自包含端点 + `ICommand/IQuery` + `[EventHandler]` Handler + DTO；`#include "../infra.cs"`（`#if !INFRA_CS` 守卫防重复定义）。端点全量见 `SaaS-Spec.md` §5（21 端点 / 5 组）与 `docs/openapi.yaml`。

### 2.3 CQRS 命令/查询数据流
```
[命令-写]  POST /api/v1/faults/{id}/ack → Auth → TenantMiddleware(注入 tid)
   → IMediator.Send(AckFaultCommand) → Handler
     → SaveChanges(全局过滤器强制 + 拦截器盖戳 TenantId)
     → Outbox 同事务写集成事件 → 域内事件 DeviceStateChanged → 读模型同 UoW 投影
   后台 Agent 轮询 Outbox → EMQX / Billing 计量（最终一致）

[查询-读]  GET /api/v1/devices → Auth → TenantMiddleware
   → IMediator.Send(ListDevicesQuery) → Handler
     → DbContext(全局过滤器自动 WHERE TenantId=当前) → DTO → 200

[实时告警]  FaultAcked 域内事件 → SignalR Hub 直推前端角标（热路径强一致，不走读副本）
```

---

## ③ 共享 DB + TenantId 隔离实现要点

- **3.1 全局过滤器 + 标记接口**：`interface ITenantEntity { Guid TenantId { get; set; } }`；`BaseDbContext.OnModelCreating` 对实现者统一 `HasQueryFilter(e => e.TenantId == _currentTenant.TenantId)`；lookup/系统表不实现则不过滤；复合索引 `(TenantId, CreatedAt)` 预建。
- **3.2 防越权写盖戳**：`SaveChangesInterceptor` 在 `SavingChanges` 阶段，对 `ITenantEntity` 且 `TenantId == default` 强制写入当前租户；与当前租户不符则抛 `CrossTenantWriteException`（框架级兜底，超开发者纪律）。
- **3.3 JWT 中间件注入**：管线 `UseRouting → UseAuthentication → TenantMiddleware → UseAuthorization`；从 `ctx.User.FindFirst("tid")` 取租户，查注册表校验存在且激活，写入 `ICurrentTenant`（**Scoped，绝不可 Singleton**）；`tid` 声明为权威，内部服务可 mTLS + `X-Tenant-Id`。
- **3.4 RLS 兜底**：`ENABLE ROW LEVEL SECURITY` + `POLICY ... USING (tenant_id = current_setting('app.tenant_id')::uuid)`；连接拦截器每连接 `SET app.tenant_id`；缓存键前缀 `tenant:{tid}:{key}`；后台作业 payload 显式带 `tid` 并 restore。

### 3.5 启动期 Schema + RLS 自动引导（Phase 3.5，提交 594ebe9）
- 应用启动在 `Program.cs` 的 `DbSeeder.SeedAsync` 之前调用 `SaasSchemaBootstrap.BootstrapAsync`：`Database.EnsureCreatedAsync()`（幂等建缺失表，预览版不引入 Migrations）+ 从磁盘读取 `sql/rls.sql` 并 `ExecuteSqlRawAsync` 执行（DO 块已幂等，每启动可重跑）。
- 引导/种子在平台服务连接下运行，Scope 内 `ICurrentTenant` 默认 `TenantId=Guid.Empty`，`TenantConnectionInterceptor` 在连接打开时 `SET app.tenant_id=Guid.Empty`；根行（平台超管 `ApplicationUser/ApplicationRole/ApplicationUserRole`）携带 `TenantId=Guid.Empty`，正好满足 RLS `WITH CHECK`，故即便非 BYPASSRLS 角色也仅写 root 行成立（详见 `infra.cs` 注释）。
- 注：`plc-saas` 为 File-based App，根目录无 `.csproj`；类型检查与 ADR-108 红线由 `plc-saas.verify` 工程承担，运行应用用 `dotnet run Program.cs`（需 PostgreSQL）。

---

## ④ File-based App 服务端/边缘落地形态与限制

- **形态**：服务端 `Program.cs` + `#include "infra.cs"`（DI/Auth/DB/中间件）+ `slices/*.cs`（每切片 `#include`）；NuGet 引用集中在 `infra.cs` 用 `#r "nuget:..."`；运行 `dotnet run Program.cs`。边缘沿用 `edge-aot-host.cs`（NativeAOT 单文件）。.NET 11 `#include` 是多文件 VSA 关键使能。
- **限制**：(1) 无 `.csproj`，细粒度构建配置靠预处理指令 + `#include`；(2) IDE 需 VS 2026 Insiders / VS Code + C# Dev Kit + .NET 11；(3) 测试建议 project-based 工程（xUnit v3 on MTP）引用切片源；(4) NativeAOT 仅边缘，服务端 JIT（EF Core/Identity 不利 AOT）；(5) 部分依赖 `.csproj` 的分析器/覆盖率需转 project 形态。

---

## ⑤ 不可行警告与风险清单

1. **.NET 11 Preview.6 非生产级**：GA 前 API 可能变；钉 preview.6、规避 Bleeding-edge、上线前迁 GA、CI 锁版本。
2. **EF Core 11 / Npgsql preview 兼容**：钉同版本、集成测试先行、保留回退 .NET 10 LTS 评估。
3. **File-based 调试/测试/IDE 限制**（见 ④）：需明确"测试用 project 工程"约定。
4. **CQRS 最终一致性对告警实时性**：热路径走同 UoW 投影 + SignalR 直推，仅分析查询走读副本。
5. **全局过滤器被 `IgnoreQueryFilters()` 绕过**：拦截器 + RLS + 审计；跨租户仅 `tid=root` 超管且显式策略。
6. **V1 含租户内基础四角色**（与 SaaS-PRD MVP 一致）：管理员 / 分析师 / 值守 / 设备工程师；分析师强制只读无写入口。租户间硬隔离不变；企业 SSO / LDAP / 外部目录列入 Later。角色模型落到 Identity 上下文（`ApplicationUser.TenantId` 已就位，角色表本期建设）。
7. **Billing 最终一致延迟**：MVP 接受，SLA 后续强化。
8. **边缘 Datasync 弱网冲突**：设备注册表/配置类冲突策略（PRD 开放问题 3）需在 Tenant 迁移阶段定。

---

## ⑥ ADR 注册表（摘要）

| ADR | 标题 | 决策 | 状态 |
|-----|------|------|------|
| 100 | .NET 11 SDK 版本钉选 | 钉 `11.0.100-preview.6`，rollForward=latestPatch | Accepted |
| 101 | VSA 端点组织 | Minimal API 内联 + File-based `#include`，不引 Carter | Accepted |
| 102 | CQRS 派发 | Mediator(MIT) 源码生成；Outbox 自建事务表（MVP 不引 Wolverine） | Accepted |
| 103 | 多租户隔离 | 共享 DB + TenantId + EF 全局过滤器 + RLS 兜底 | Accepted |
| 104 | 认证 | ASP.NET Core Identity + JWT（access 15min / refresh 7d，`tid` 声明） | Accepted |
| 105 | 告警实时性 | 同 UoW 投影 + SignalR 直推，读副本仅分析查询 | Accepted |
| 106 | 图标库（P0） | 锁定 Lucide (ISC)，禁 emoji | Accepted |
| 107 | MQTT 多租户隔离 | 方案 B：租户级 MQTT 凭据 + Broker ACL，九段主题不变 | Accepted |
| 108 | Billing MVP 范围 | 仅计量 + 配额强制，不计费、不接支付、无货币字段（内网免费，AC-10） | Accepted |

完整 MADR 见 `docs/decisions/ADR-100-107.md`（含 ADR-108）。

---

## ⑦ ADR-107：MQTT 多租户隔离（回应 PM 依赖：九段主题无租户段）

### 背景
既有九段主题 `devices/{continent}/{country}/{province}/{city}/{district}/{line}/{batch}/{deviceId}/{type}`（§17.8 / §17.9）**不含租户段**。PM 提出需在多租户下隔离 MQTT 流量。需选择：方案 A（主题加 `t/{tenantId}` 前缀）或方案 B（租户级 MQTT 凭据 + Broker ACL，九段主题不变）。

### 关键事实
- `DeviceId` 为聚合根全局唯一标识（跨租户唯一），故设备与其租户的归属可在服务端注册表（`Tenant`/`Device` 注册）中维护。
- 中心 Broker 为 EMQX；边缘为 Mosquitto 经 `bridge` 转发至 EMQX（架构 V9 §17.9）。

### 方案对比
| 维度 | A. 主题加 `t/{tenantId}` 前缀 | B. 租户级凭据 + Broker ACL（九段不变） |
|------|-------------------------------|----------------------------------------|
| 对 §17.8/§17.9 冲击 | 大：所有发布/订阅/前端 Mqtt.js/`TopicPath` 值对象需改造 | 无：九段主题与既有体系完全保留 |
| 边缘/前端改造 | 需改边缘桥接、EMQX 路由、前端订阅、域模型 `TopicPath` | 仅连接凭据带租户身份，主题树不变 |
| 隔离强制点 | 主题前缀（应用层约定，易被误发错前缀） | Broker 层 ACL（服务端强制，无法绕过） |
| 租户标识暴露 | 主题树含 `tenantId`（轻微泄露） | 凭据内 `tid`，主题树不含 |
| 隔离粒度 | 租户级（前缀） | 租户级 → 可细化到 deviceId（因全局唯一） |

### 决策
**采用方案 B**：租户级 MQTT 凭据 + Broker ACL，九段主题不变。

### 落地要点（B）
- 每个 MQTT 连接携带租户身份：浏览器经用户 JWT 派生的短时 MQTT 凭据（含 `tid`）；边缘网关在 onboard 时获租户作用域凭据（存 Device 注册表，可轮换）；边缘 Mosquitto→EMQX 桥接用该租户凭据。
- EMQX 启用 **JWT 认证**（校验我们 IdP 签发、含 `tid` 的 JWT）+ **AuthZ ACL**：ACL 规则引用连接 `tid`，经服务端 `Tenant`/`Device` 注册表将"连接 tid ↔ 其名下 deviceId 集合"映射为允许的主题过滤器 `devices/+/+/+/+/+/+/{deviceId}/#`。EMQX HTTP AuthZ 端点（`/mqtt/acl`）按 `tid` 返回该租户设备主题白名单。
- 因 `DeviceId` 全局唯一，即使无租户段，租户 A 也只能访问其名下 deviceId 的主题，租户间完全隔离。
- MVP 可先放宽到"租户级允许其全部 devices 主题"；per-device 细化在 Later。

### 后果
- 正面：不打断 §17.8/§17.9 既有九段体系与边缘/前端代码；隔离在 Broker 层强制，前端/边缘零改造；租户维度经凭据传入，主题树干净。
- 负面：需 EMQX JWT auth + HTTP AuthZ ACL 与 `Tenant`/`Device` 注册表联动；凭据生命周期与轮换需设计（边缘网关凭据轮换）。
- 假设：一个边缘站点（厂区）归属单一租户；多租户边缘站点（一个网关服务多租户）列入 Later，届时需多桥接或多凭据。
- 关联：ADR-103（共享 DB 租户隔离）、ADR-104（JWT 含 `tid`）、ADR-105（SignalR 实时推送与 MQTT 解耦）。
