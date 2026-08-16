# Spec - PLC·AIOT 多租户 SaaS v1.0

> 生成日期：2026-08-16
> 基于：SaaS-PRD v0.1 + Arch-Spec v1.0 + DESIGN.md §10（SSOT）
> 状态：已确认（用户 Phase 1 三文档确认 + Billing 范围裁定）
> 本 Spec 为规格即契约（Spec-as-Contract），Phase 2/3/4 全部以此为唯一依据。

---

## 1. 产品定义

- **一句话描述**：在现有 PLC·AIOT 内网控制台基础上**增量**建设多租户 SaaS 层（Tenant / Identity / Billing 三限界上下文），实现租户自助开通、行级隔离、租户内 RBAC、套餐配额计量与运营暂停。
- **目标用户**：外部中小制造企业（租户管理员）、平台运营（超管）、租户内分析师 / 值守 / 设备工程师。
- **核心问题**：单租户内网控制台无法面向多外部租户售卖与隔离；缺多租户认证、角色模型与用量计量；边缘 / MQTT 层无租户维度。
- **本期关键裁定（用户 2026-08-16）**：Billing 上下文落地「套餐档（配额差异化）+ 用量计量 + 配额强制」，但**只计量、不产生费用、不接支付**——内网免费使用。订阅生命周期仅保留运营态；MRR / NRR 商业北极星本期不适用（详见 §2/§3/§9）。

---

## 2. MVP 范围（锁定——不在此列表的功能一律不做）

| 优先级 | 功能（垂直切片） | 验收标准摘要 | RICE | 计费相关 |
|--------|------------------|--------------|------|----------|
| P0 | 租户开通与自动预置（Tenant 上下文） | 5 分钟内建租户 + 预置 TenantId + 默认管理员 | 3.75 | — |
| P0 | 租户隔离强制（中心库 + MQTT / 边缘层） | 跨租户访问 100% 拦截 | 3.75 | — |
| P0 | 多租户登录与租户内角色（Identity） | 四角色授权，分析师只读 | 3.33 | — |
| P1 | 套餐与配额模型 + 用量计量（Billing） | 套餐档可切、用量实时、配额可 enforced | 1.60 | **只计量不收费** |
| P1 | 订阅生命周期（运营态：Active + 平台暂停） | 平台可暂停 / 恢复租户，数据保留 | 2.56 | 财务逾期→付费延后 |
| P1 | 租户管理控制台（账户 4 页） | 用量 / 套餐 / 成员 / 账单只读可见 | 2.52 | — |
| P1 | 平台运营后台最小可用（5 页） | 组织 / 计费策略 / 全局角色 / 审计 / 健康 | 0.90 | — |
| P1 | 配额超限提醒与限流（G5/G8） | ≥90% 提醒，超限额流 | — | — |

> **Phase 0 决策落地**：Tenant / Identity / Billing 三上下文同版本完整建设。前三为绝对前置；后五为同发布内紧随的垂直切片，不进 Backlog。仅支付网关、审计、SSO 进 Backlog。

---

## 3. 明确不做（Out-of-Scope — 锁定）

| 不做的功能 | 原因 | 何时考虑 |
|------------|------|----------|
| **计费出账 / 支付网关 / 发票** | 用户裁定本期内网免费、只计量不收费 | 计费上线（Later），届时补出账 / 支付 / 发票 |
| 真实货币 MRR / NRR 商业化指标 | 免费使用无可计费收入 | 计费上线后启用 |
| 白标（自定义 Logo / 域名 / 品牌色） | MVP 单品牌（科技天蓝） | 企业版路线图 |
| 私有化部署 / 物理库隔离 | 共享库行级为默认 | 企业版路线图 |
| SSO / LDAP / 企业目录（OIDC） | 租户内基础四角色已满足 MVP | 企业版（Later） |
| 审计日志（租户内 + 平台） | 范围控制 | Backlog（Phase 4 前补齐） |
| 试用→逾期→付费财务转换 | 免费使用无付费环节 | 计费上线后 |

---

## 4. 技术架构（锁定——含版本锚定）

> 版本锚定：框架写实际版本号，防止幻觉 API。技术栈由架构师选型（ADR-100~107），专家团不预设。

| 层 | 技术 / 选型 | 实际版本 | 锁定原因 |
|----|-------------|----------|----------|
| 运行时 | .NET 11 Preview | SDK `11.0.100-preview.6`（C# 14） | ADR-100，钉 preview.6 + rollForward=latestPatch |
| 架构范式 | VSA（Vertical Slice）+ DDD 清洁 + CQRS | — | Phase 0 锁定；端点内联 + File-based `#include` |
| 端点组织 | Minimal API 内联 + `#include "infra.cs"` | — | ADR-101，零程序集扫描 |
| CQRS 派发 | Mediator（martinothamar, MIT）源码生成 | 3.x | ADR-102，零反射 / NativeAOT 友好 |
| Outbox | 自建事务表 + 轮询 Agent | — | ADR-102，不引 Wolverine |
| 数据库 | PostgreSQL（共享库） | PG 16+ | ADR-103 |
| ORM | EF Core | `11.0.0-preview.6` | ADR-100/103 |
| 驱动 | Npgsql | `11.0.0-preview.6` | 同上 |
| 隔离 | TenantId 行级 + EF 全局过滤器 + RLS 兜底 | — | ADR-103 |
| 认证 | ASP.NET Core Identity + JWT（access 15min / refresh 7d，`tid` 声明） | — | ADR-104 |
| 实时 | SignalR Hub 直推告警 | — | ADR-105 |
| MQTT 隔离 | 方案 B：租户级凭据 + EMQX JWT auth + HTTP AuthZ ACL | — | ADR-107，九段主题不变 |
| 图标 | Lucide（ISC，24×24，2px，currentColor） | — | ADR-106，全站禁 emoji |
| 前端形态 | File-based App（`Program.cs` 顶层 + `#include`）；边缘沿用 NativeAOT 宿主 | — | 服务端 JIT / 边缘 AOT |

---

## 5. API 端点清单（锁定——开发唯一依据）

> 架构师 Phase 2 据此产出 `openapi.yaml`（OpenAPI 3.2）。下表为锁定契约，新增端点须走变更流程。

### 5.1 租户（Tenant 上下文）
| Method | Path | 功能 | 认证 | 说明 |
|--------|------|------|------|------|
| POST | `/api/v1/tenants` | 自助开通 + 自动预置（TenantId / 配额 / 默认管理员） | 匿名（落地页） | G1 |
| GET | `/api/v1/tenants/{id}` | 租户详情（平台运营） | 超管（`tid=root`） | — |
| POST | `/api/v1/ops/tenants/{id}/suspend` | 平台暂停租户（运营态） | 超管 | G7 |
| POST | `/api/v1/ops/tenants/{id}/resume` | 平台恢复租户 | 超管 | G7 |

### 5.2 身份（Identity 上下文）
| Method | Path | 功能 | 认证 | 说明 |
|--------|------|------|------|------|
| POST | `/api/v1/auth/login` | 租户标识 + 账号登录，签发 JWT（`tid`） | 匿名 | G3 |
| POST | `/api/v1/auth/refresh` | refresh 换 access | refresh token | — |
| GET | `/api/v1/tenant/members` | 列租户内成员与角色 | 租户用户 | — |
| POST | `/api/v1/tenant/members` | 添加成员并分配角色 | 管理员 | G4 |
| POST | `/api/v1/tenant/members/{id}/role` | 改成员角色 | 管理员 | 四角色内 |
| POST | `/api/v1/mqtt/credential` | 用 JWT 派生短时 MQTT 凭据（含 `tid`） | 租户用户 | ADR-107 |

### 5.3 计费 / 计量（Billing 上下文——只计量不收费）
| Method | Path | 功能 | 认证 | 说明 |
|--------|------|------|------|------|
| GET | `/api/v1/billing/usage` | 本租户设备数 / 遥测用量 / 配额剩余 | 租户用户 | G5 |
| GET | `/api/v1/billing/plan` | 当前套餐档 | 租户用户 | — |
| POST | `/api/v1/billing/plan` | 切换套餐档（免费档间） | 管理员 | quota 变化 |
| GET | `/api/v1/billing/quota` | 配额阈值与超限策略 | 租户用户 | G8 |

### 5.4 既有 PLC 域（全部带 TenantId，沿用 V9）
| Method | Path | 功能 | 认证 | 说明 |
|--------|------|------|------|------|
| GET | `/api/v1/devices` | 设备列表（按租户过滤） | 租户用户 | G2 |
| POST | `/api/v1/faults/{id}/ack` | 确认告警（分析师拒绝） | 租户用户（非分析师） | G4 |
| GET | `/api/v1/faults/stream` | SignalR 实时告警角标 | 租户用户 | ADR-105 |

### 5.5 平台运营（超管）
| Method | Path | 功能 | 认证 |
|--------|------|------|------|
| GET | `/api/v1/ops/tenants` | 全部租户 + 健康 | 超管 |
| GET | `/api/v1/ops/pricing` | 计费策略 / 套餐档定义 | 超管 |
| GET | `/api/v1/ops/roles` | 全局角色与权限矩阵 | 超管 |
| GET | `/api/v1/ops/health` | 平台健康（SLO） | 超管 |

---

## 6. 数据库表清单（锁定）

| 表名 | 核心字段 | 索引 | 关联 |
|------|----------|------|------|
| `Tenants` | Id(GUID, PK), Name, Slug(唯一), Status(Active/Suspended), CreatedAt | Slug 唯一 | 1:多 Subscriptions |
| `AspNetUsers`（`ApplicationUser : IdentityUser`） | Id, TenantId(GUID), UserName, ... | (TenantId) | ITenantEntity |
| `AspNetRoles` / `AspNetUserRoles` | 四角色（Admin/Analyst/Operator/DeviceEng） | — | Identity |
| `Plans` | Id, Name(免费/专业/商业/企业), DeviceQuota, TelemetryQuota | — | 套餐档（配额差异化，**无价格字段**） |
| `Subscriptions` | Id, TenantId, PlanId, Status(Active/Suspended), StartAt | (TenantId) | 租户↔套餐 |
| `UsageMeters` | Id, TenantId, Period(月), DeviceCount, TelemetryPoints | (TenantId, Period) | 计量 |
| `QuotaPolicies` | Id, PlanId, ThresholdPct, Action(Warn/Throttle) | — | G5/G8 |
| `Devices` / `FaultEvents` / `Telemetry*` | 既有 + **TenantId** 列 | (TenantId, CreatedAt) | 实现 ITenantEntity |
| `OutboxMessages` | Id, TenantId(payload 显式), Type, Payload, SentAt | (SentAt) | ADR-102 |
| `AuditLogs` | Id, TenantId(nullable=平台), Actor, Action, At | (TenantId, At) | Backlog 启用 |

> 所有租户作用域实体实现 `ITenantEntity`；`BaseDbContext` 统一全局过滤器 + `SaveChangesInterceptor` 盖戳（ADR-103）。

---

## 7. 页面清单（锁定）

| 页面 | 路由 | 核心组件 | 对应 API | 设计 Token |
|------|------|----------|----------|------------|
| 落地页开通 | `/` | 开通表单（企业信息） | POST /tenants | DESIGN §17 落地页 |
| 租户切换器（顶栏常驻） | 顶栏 | 组织标 + 租户名 + 展开 + 当前数据范围边界条 | — | --tenant-dot(brand-500) |
| 账户·用量概览 | `#account/usage` | .kpi + .meter(role=progressbar) | GET /billing/usage | 玻璃态 / 暗色 |
| 账户·套餐与订阅 | `#account/plan` | .badge.info(档位) + 切换 | GET/POST /billing/plan | 单品牌 |
| 账户·成员管理 | `#account/members` | .tbl + 角色下拉 | GET/POST /tenant/members | WCAG AA |
| 账户·账单与发票 | `#account/billing` | .tbl(只读，本期无账单) | — | 真实文案占位 |
| 运营·组织管理 | `#ops/tenants` | .tbl + 暂停/恢复 | GET/POST /ops/tenants | 超管 |
| 运营·计费策略 | `#ops/pricing` | 套餐档定义表 | GET /ops/pricing | — |
| 运营·全局角色 | `#ops/roles` | 权限矩阵 | GET /ops/roles | — |
| 运营·审计日志 | `#ops/audit` | .tbl | GET /ops/audit | Backlog 启用 |
| 运营·平台健康 | `#ops/health` | SLO 看板 | GET /ops/health | — |
| 既有 8 页（Dashboard/Devices/Alarms/Statistics/Realtime/Map/System/Admin） | 既有路由 | 复用 .card/.drawer/.badge | 既有 + TenantId | 租户作用域化 |

> 复用组件：`.card/.drawer/.badge/.meter/.kpi/.tbl/.seg/.btn/.switch`；图标统一 Lucide（16/20/24px）；导航独立分组「账户 / 运营后台」，不混入运营监控三组。

---

## 8. 设计 Token（锁定）

> 设计师 Phase 2 据此产出 `design-tokens.json` + `design-tokens.css`，前端 import 引用。完整 SSOT 见 DESIGN.md §10。

- **主色**：科技天蓝 `--brand-500: #3b82f6`；主按钮 `--brand-600: #2563eb`（WCAG AA 4.6:1）
- **暗色背景**：`--bg: #020617`；玻璃态 `backdrop-filter: blur(14px) saturate(140%)`
- **语义色**：ok / warn / bad / info（徽章文字用 AA 安全深色）
- **字体**：Inter + Noto Sans SC
- **图标库**：Lucide（ISC，24×24，2px 描边，currentColor）；雪碧图 `wwwroot/css/icons.svg`（`<symbol>` + `<use href="#i-xxx"/>`）；尺寸令牌 16/20/24px
- **对标品牌**：Datadog / Grafana / Linear / Samsara（暗色高密度数据控制台）
- **单品牌约束**：禁第二主色（租户身份用 `--tenant-dot: var(--brand-500)` 文字 + 组织标；套餐档用 `.badge.info` 文字区分，不上色）；**禁紫→粉渐变**；**禁硬编码色**（例外 `#fff`/`#000`）；**禁 AI 模板味文案**；状态「色 + 字」双信号

---

## 9. 验收标准（EARS 格式——QA 唯一依据）

| 编号 | 功能 | EARS 验收标准 | 优先级 |
|------|------|--------------|--------|
| AC-01 | 开通 | When 访客提交合法开通表单，系统**必须**在 5 分钟内创建租户、预置 TenantId 与默认管理员并返回登录入口 | P0 |
| AC-02 | 隔离 | When 租户 A 用户查询，系统**必须**仅返回 TenantId=A 的数据，且响应头带租户上下文标识 | P0 |
| AC-03 | 隔离 | If 租户 A 用户访问带租户 B 设备 ID 的资源，系统**必须**返回 403 且不泄露 B 的任何字段 | P0 |
| AC-04 | RBAC | When 分析师登录并进入设备「指令」Tab，该 Tab**必须**不可交互（无写入口），仅只读呈现 | P0 |
| AC-05 | 计量 | While 租户用量达配额 90%，计量任务运行后系统**必须**向管理员推送提醒并记录 `quota_threshold_reached` | P1 |
| AC-06 | 配额 | If 用量超配额，系统**必须**按策略限流（而非静默丢弃）并记录 `quota_exceeded` | P1 |
| AC-07 | 运营暂停 | When 平台运营暂停租户，该租户读写**必须**被拒（读降级只读快照），数据保留 | P1 |
| AC-08 | 实时 | When 告警被确认，SignalR**必须**在 1s 内推前端角标更新 | P1 |
| AC-09 | MQTT | While 租户 A 连接，EMQX ACL**必须**仅允许其名下 deviceId 主题，租户间**必须**隔离 | P0 |
| AC-10 | 计费裁定 | While 系统计量与展示用量 / 配额，系统**必须不**产生任何费用、账单或支付流程（内网免费） | P0 |
| AC-11 | 可达性 | When 渲染配额 meter，系统**必须**输出 `role=progressbar` + `aria-valuenow/max` | P2 |

---

## 10. 边界与约束

- 空状态：新租户无设备 → 引导「注册首台设备」，不空白报错。
- 隔离失败兜底：TenantId 缺失的写命令在中间件层拒绝，不落库。
- 配额超限：超配额注册 / 遥测按策略限流或计入超额（不静默丢弃）。
- 并发开通：同名子域 → 唯一性校验失败并提示换名。
- 离线 / 弱网：边缘断网本地 SQLite 落库，恢复后经 Datasync 按租户增量回推，冲突按既定策略收敛。
- 权限拒绝：跨租户 / 无角色 → 403 + 审计，前端引导联系租户管理员。
- **免费使用约束**：所有套餐档仅作配额差异化，无价格；无支付 / 发票 / 出账链路；计费上线前不得引入货币字段与支付调用。
- 性能：首屏 ≤3s（公网）；租户内 API p95 ≤500ms；开通预置 ≤5 分钟。
- 安全：所有读写经 TenantId 中间件强制；DB 行级 + MQTT ACL 双层；JWT 含 `tid`；速率限制按租户配额。
- 兼容性：Chrome/Safari/Firefox 最新 2 版；内网 Edge/Mosquitto 既有协议不变。
- 数据主权：共享库行级默认；物理隔离 / 私有部署进企业版路线图。

---

## 11. 内嵌已知坑（Phase 3 开发前由 lead 召回）

| 坑 | 技术栈指纹 | 根因 | 修法 |
|----|------------|------|------|
| .NET 11 preview API 漂移 | net11-preview.6 | preview 期 API 可变 | 钉 preview.6；上线前迁 GA；CI 锁版本（ADR-100） |
| EF 全局过滤器被 `IgnoreQueryFilters()` 绕过 | ef-core | 显式忽略 | 拦截器 + RLS + 审计；跨租户仅 `tid=root` 显式策略（ADR-103） |
| File-based 测试形态 | file-based-app | 无 .csproj | 测试用 project 工程（xUnit v3 on MTP）引用切片源（ADR-101） |
| **P0 阻塞：emoji → Lucide 替换** | 既有 wwwroot 前端 | 现有前端大量 emoji 作图标 | Phase 3 前置：全站替换 emoji 为 Lucide 雪碧图（ADR-106） |
| 告警最终一致滞后 | cqrs-outbox | 读副本延迟 | 热路径同 UoW 投影 + SignalR 直推（ADR-105） |

> 正式 `pitfalls.jsonl` 由 Phase 3 开发开始时建立；本表为 Phase 1 已知项预登记。

---

## 12. 端到端验证步骤（Spec 锁定最后一项）

```bash
# 1. 构建（测试用 project 工程引用切片源）
dotnet build -c Release

# 2. 启动（File-based 服务端）
dotnet run Program.cs   # 等待 "Now listening on http://localhost:5000"

# 3. 核心成功流：自助开通
curl -X POST http://localhost:5000/api/v1/tenants -H "Content-Type: application/json" \
  -d '{"companyName":"测试制造","slug":"test-mfg","adminEmail":"admin@test.com"}'
# 断言：201 + TenantId + 默认管理员账号

# 4. 多租户登录
curl -X POST http://localhost:5000/api/v1/auth/login -H "Content-Type: application/json" \
  -d '{"tenant":"test-mfg","email":"admin@test.com","password":"..."}'
# 断言：200 + JWT（含 tid 声明）

# 5. 跨租户隔离（关键错误流）
curl http://localhost:5000/api/v1/devices/{其他租户设备ID} -H "Authorization: Bearer $TOKEN"
# 断言：403，且不泄露其他租户字段

# 6. 计量只计量不收费（裁定验证）
curl http://localhost:5000/api/v1/billing/usage -H "Authorization: Bearer $TOKEN"
# 断言：200 + 设备数/遥测用量/配额剩余；响应中无任何金额/账单字段

# 7. 分析师只读（关键错误流）
# 以分析师角色登录后调用 POST /api/v1/faults/{id}/ack
# 断言：403，指令入口不可达
```

---

## 13. 变更记录

| 日期 | 变更内容 | 原因 | 影响范围 |
|------|----------|------|----------|
| 2026-08-16 | 生成 Spec v1.0（Phase 1.5） | 用户确认三文档 + Billing 范围裁定 | 全文 |
| 2026-08-16 | Billing 裁定：套餐/配额/用量计量上线但只计量不收费（内网免费） | 用户明确内网免费使用 | §1/§2/§3/§9(AC-10)/§10 |
| 2026-08-16 | ADR-108 登记（Billing MVP=计量+配额强制，不计费） | 同上，架构决策留痕 | docs/decisions/ADR-100-107.md |

---

## 附录：关联文档与决策索引

- 产品：SaaS-PRD v0.1（竞品 / 用户故事 / RICE / 北极星）
- 架构：docs/SaaS-Architecture-Spec.md v1.0（选型矩阵 / 隔离实现 / MQTT 方案 B）
- 设计：DESIGN.md §10（SaaS 层设计系统 SSOT）
- 决策：docs/decisions/ADR-100-107.md（ADR-100~107）+ ADR-108（Billing 计量裁定）
- 既有：docs/PLC-IoT-Architecture-Complete.md（V9 领域模型 / 九段 MQTT / 边缘链路）
