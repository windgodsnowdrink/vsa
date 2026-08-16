# PLC·AIOT 多租户 SaaS — 数据库 Schema（Phase 2）

> 依据：SaaS-Spec §6（表清单，锁定） + ADR-103（隔离） + ADR-108（Billing 只计量不收费）
> 数据库：PostgreSQL 16+（共享库，行级隔离）。ORM：EF Core 11.0.0-preview.6 + Npgsql。
> 本文件为开发建表 / 迁移唯一依据；新增表或字段须走变更流程。

---

## 1. 命名与约定

- 所有表蛇形复数（`tenants` / `usage_meters`）；主键 `id` 为 `uuid`（默认 `gen_random_uuid()`）。
- 时间统一 `timestamptz`（UTC）；金额 / 货币字段**一律不存在**（ADR-108）。
- 租户作用域实体实现 `ITenantEntity`（含 `tenant_id uuid NOT NULL`），由拦截器盖戳 + 全局过滤器隔离。
- 目录表（套餐、配额策略）非租户作用域，不实现 `ITenantEntity`，全租户共享。
- 复合索引统一 `(tenant_id, created_at)`；外键级联策略见各表。

---

## 2. ITenantEntity 盖戳机制（防越权写）

`ITenantEntity` 标记接口：

```csharp
public interface ITenantEntity
{
    Guid TenantId { get; set; }
}
```

`BaseDbContext.OnModelCreating` 对所有实现者统一注册全局查询过滤器：

```csharp
modelBuilder.Entity<Device>().HasQueryFilter(d => d.TenantId == _currentTenant.TenantId);
// 其余 ITenantEntity 实体同理；目录表（Plans / QuotaPolicies）不注册
```

`SaveChangesInterceptor` 在 `SavingChanges` 阶段强制盖戳并拒绝跨租户写入（框架级兜底，超开发者纪律）：

```csharp
public sealed class TenantStampInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentTenant _tenant;
    public override ValueTask<InterceptionResult<int>> SavingChanges(
        DbContextEventData data, InterceptionResult<int> result, CancellationToken ct)
    {
        var ctx = (BaseDbContext)data.Context!;
        foreach (var entry in ctx.ChangeTracker.Entries<ITenantEntity>()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified))
        {
            if (entry.Entity.TenantId == Guid.Empty)
                entry.Entity.TenantId = _tenant.TenantId;                 // 自动盖戳
            else if (entry.Entity.TenantId != _tenant.TenantId)
                throw new CrossTenantWriteException(                      // 跨租户拒绝
                    entry.Entity.GetType().Name, entry.Entity.TenantId, _tenant.TenantId);
        }
        return base.SavingChanges(data, result, ct);
    }
}
```

`ICurrentTenant` 为 **Scoped**（绝不可 Singleton）；值由 `TenantMiddleware` 从 JWT `tid` 声明注入（ADR-103 / ADR-104）。

---

## 3. PostgreSQL RLS 兜底（纵深防御）

即便代码误用 `IgnoreQueryFilters()` 或走原始 SQL，RLS 仍强制隔离。每个租户作用域表启用：

```sql
ALTER TABLE "devices" ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation ON "devices"
  USING (tenant_id = current_setting('app.tenant_id')::uuid);
-- 其余 ITenantEntity 表（fault_events / usage_meters / subscriptions / asp_net_users …）同法
```

连接拦截器在每条连接打开时设置当前租户（请求级）：

```csharp
public sealed class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ICurrentTenant _tenant;
    public override ValueTask<InterceptionResult> ConnectionOpenedAsync(
        DbConnection c, ConnectionEndEventData d, InterceptionResult r, CancellationToken ct)
    {
        using var cmd = c.CreateCommand();
        cmd.CommandText = "SET app.tenant_id = @tid;";
        cmd.Parameters.AddWithValue("tid", _tenant.TenantId);
        cmd.ExecuteNonQuery();
        return base.ConnectionOpenedAsync(c, d, r, ct);
    }
}
```

- 平台超管（`tid=root`）使用具 `BYPASSRLS` 权限的服务角色连接，RLS 对其透明跳过；租户作用域连接设置 `app.tenant_id`。
- 后台作业（计量 / 报表）必须在 payload 显式携带 `tenant_id`，执行前 restore `ICurrentTenant` 并 `SET app.tenant_id`。

---

## 4. 表结构定义

### 4.1 Tenants（租户根，非 ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | 租户 ID |
| name | text | | 否 | | 企业名称 |
| slug | text | UNIQUE | 否 | | 唯一标识（子域 / 路由） |
| status | text | CHECK IN ('Active','Suspended') | 否 | 'Active' | 运营态 |
| created_at | timestamptz | | 否 | now() | |

索引：`UNIQUE (slug)`。

### 4.2 AspNetUsers（ApplicationUser : IdentityUser，ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | 用户 ID |
| tenant_id | uuid | FK→tenants(id) | 否 | | ITenantEntity |
| user_name | text | | 可 | | |
| normalized_user_name | text | | 可 | | |
| email | text | | 可 | | |
| normalized_email | text | | 可 | | |
| email_confirmed | boolean | | 否 | false | |
| password_hash | text | | 可 | | |
| security_stamp | text | | 可 | | |
| concurrency_stamp | text | | 可 | | |
| phone_number | text | | 可 | | |
| phone_number_confirmed | boolean | | 否 | false | |
| two_factor_enabled | boolean | | 否 | false | |
| lockout_end | timestamptz | | 可 | | |
| lockout_enabled | boolean | | 否 | false | |
| access_failed_count | integer | | 否 | 0 | |
| created_at | timestamptz | | 否 | now() | |

索引：`(tenant_id)`；`UNIQUE (tenant_id, normalized_email)`（租户内邮箱唯一）。
FK：`tenant_id → tenants(id) ON DELETE RESTRICT`。

### 4.3 AspNetRoles（租户内角色，ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | 角色 ID |
| tenant_id | uuid | FK→tenants(id) | 否 | | ITenantEntity（角色租户内） |
| name | text | | 可 | | Admin/Analyst/Operator/DeviceEng |
| normalized_name | text | | 可 | | |
| concurrency_stamp | text | | 可 | | |

索引：`(tenant_id)`；`UNIQUE (tenant_id, normalized_name)`。
四角色由种子数据按租户预置；分析师强制只读（指令入口不可达，AC-04）。

### 4.4 AspNetUserRoles（ITenantEntity，冗余 tenant_id 便于过滤）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| user_id | uuid | PK 部分，FK→asp_net_users(id) | 否 | | |
| role_id | uuid | PK 部分，FK→asp_net_roles(id) | 否 | | |
| tenant_id | uuid | FK→tenants(id) | 否 | | ITenantEntity |

PK：`(user_id, role_id)`。FK：`user_id→asp_net_users(id)`，`role_id→asp_net_roles(id)`，`tenant_id→tenants(id)`。
索引：`(tenant_id)`。

### 4.5 RefreshTokens（refresh 存储，ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | |
| user_id | uuid | FK→asp_net_users(id) | 否 | | |
| tenant_id | uuid | FK→tenants(id) | 否 | | ITenantEntity |
| token_hash | text | | 否 | | refresh token 哈希（不存明文） |
| expires_at | timestamptz | | 否 | | 7 天 |
| revoked_at | timestamptz | | 可 | | 吊销时间 |

索引：`(tenant_id)`；`(user_id)`。FK：`user_id→asp_net_users(id) ON DELETE CASCADE`。

### 4.6 Plans（套餐档，**无价格字段**，目录表非 ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | |
| name | text | CHECK IN ('免费','专业','商业','企业') | 否 | | 套餐档（仅配额差异化） |
| device_quota | integer | | 否 | | 设备配额上限 |
| telemetry_quota | integer | | 否 | | 遥测点配额上限（周期） |
| created_at | timestamptz | | 否 | now() | |

索引：`UNIQUE (name)`。
**红线**：不得添加 price / currency / amount / cost 等任何货币字段（ADR-108 / AC-10）。

### 4.7 Subscriptions（租户↔套餐，ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | |
| tenant_id | uuid | FK→tenants(id) | 否 | | ITenantEntity |
| plan_id | uuid | FK→plans(id) | 否 | | 当前套餐档 |
| status | text | CHECK IN ('Active','Suspended') | 否 | 'Active' | 运营态（与租户同步暂停） |
| start_at | timestamptz | | 否 | now() | |
| end_at | timestamptz | | 可 | | 预留（免费期无结束） |
| created_at | timestamptz | | 否 | now() | |

索引：`(tenant_id)`；`UNIQUE (tenant_id)`（每租户一条当前订阅）。
FK：`tenant_id→tenants(id)`，`plan_id→plans(id)`。

### 4.8 UsageMeters（计量，ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | |
| tenant_id | uuid | FK→tenants(id) | 否 | | ITenantEntity |
| period | text | | 否 | | 计费月 YYYY-MM |
| device_count | integer | | 否 | 0 | 当期设备数 |
| telemetry_points | bigint | | 否 | 0 | 当期遥测点累计 |
| created_at | timestamptz | | 否 | now() | |
| updated_at | timestamptz | | 否 | now() | |

索引：`(tenant_id, period)`；`UNIQUE (tenant_id, period)`。
> `quota_remaining` 为派生值（套餐配额 − 用量），不存储金额；见 openapi.yaml `UsageResponse`。

### 4.9 QuotaPolicies（配额策略，目录表非 ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | |
| plan_id | uuid | FK→plans(id) | 否 | | 关联套餐 |
| metric | text | CHECK IN ('device','telemetry') | 否 | | 计量维度 |
| threshold_pct | numeric(5,2) | | 否 | | 阈值百分比（如 90） |
| action | text | CHECK IN ('Warn','Throttle') | 否 | | 超限动作 |

索引：`(plan_id)`。FK：`plan_id→plans(id) ON DELETE CASCADE`。

### 4.10 Devices（既有 + TenantId，ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | DeviceId（全局唯一） |
| tenant_id | uuid | FK→tenants(id) | 否 | | ITenantEntity |
| name | text | | 否 | | |
| status | text | CHECK IN ('online','offline','fault') | 否 | 'offline' | |
| profile | jsonb | | 可 | | DeviceProfile 点表 |
| last_heartbeat | timestamptz | | 可 | | |
| created_at | timestamptz | | 否 | now() | |

索引：`(tenant_id, created_at)`；`(tenant_id)`。
FK：`tenant_id→tenants(id)`。说明：因 `id` 全局唯一，MQTT 隔离（ADR-107）按 device_id 映射即可区分租户。

### 4.11 FaultEvents（既有 + TenantId，ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | |
| tenant_id | uuid | FK→tenants(id) | 否 | | ITenantEntity |
| device_id | uuid | FK→devices(id) | 否 | | |
| code | text | | 否 | | 故障码 |
| severity | text | | 否 | | |
| status | text | CHECK IN ('open','acked','resolved','closed') | 否 | 'open' | |
| correlation_id | text | | 可 | | 全链路追踪 |
| created_at | timestamptz | | 否 | now() | |

索引：`(tenant_id, created_at)`；`(tenant_id, status)`。
FK：`tenant_id→tenants(id)`，`device_id→devices(id)`。

### 4.12 Telemetry 原始数据（说明，非关系表）

高频遥测点按 V9 写入 **InfluxDB（时序库）**，不落关系表。本库仅以 `UsageMeters.telemetry_points` 存聚合计数用于配额计量。原始遥测表不在关系 Schema 内（避免巨表与行级隔离开销）。

### 4.13 OutboxMessages（事务发件箱，ITenantEntity）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | |
| tenant_id | uuid | FK→tenants(id) | 否 | | payload 显式携带（ADR-102） |
| type | text | | 否 | | 事件类型（如 TelemetryIngested / QuotaExceeded） |
| payload | jsonb | | 否 | | 事件负载 |
| sent_at | timestamptz | | 可 | | 投递后写入；NULL=待发 |
| created_at | timestamptz | | 否 | now() | |

索引：`(sent_at) WHERE sent_at IS NULL`（待发轮询）；`(tenant_id)`。
FK：`tenant_id→tenants(id)`。

### 4.14 AuditLogs（Backlog 启用，tenant_id 可空）

| 列 | 类型 | 约束 | NULL | 默认 | 说明 |
|----|------|------|------|------|------|
| id | uuid | PK | 否 | gen_random_uuid() | |
| tenant_id | uuid | FK→tenants(id) | 可 | | 平台级为 NULL |
| actor | text | | 否 | | 操作者标识 |
| action | text | | 否 | | 动作 |
| at | timestamptz | | 否 | now() | |
| detail | jsonb | | 可 | | |

索引：`(tenant_id, at)`；`(at)`。FK：`tenant_id→tenants(id) ON DELETE SET NULL`。
> 跨租户访问拒绝（403）须记审计（AC-03 / 权限拒绝）。

---

## 5. 索引与隔离一致性小结

- 所有 `ITenantEntity` 表均建 `(tenant_id, created_at)` 复合索引（或 `(tenant_id)` 变体），避免全表扫。
- 全局过滤器 + `SaveChangesInterceptor` 盖戳 + RLS 三层叠加，杜绝跨租户读 / 写泄露。
- 目录表（`plans` / `quota_policies`）无 `tenant_id`，全租户共享，过滤器不注册。
- 任一写路径在 `TenantMiddleware` 未解析到 `tid` 时于中间件层拒绝，不落库（Spec §10 兜底）。

---

## 6. 与 SaaS-Spec §6 一致性核对

| Spec §6 表 | 本文件 | 一致 |
|-----------|--------|------|
| Tenants | §4.1 | 是 |
| AspNetUsers | §4.2 | 是（含 TenantId） |
| AspNetRoles / AspNetUserRoles | §4.3 / §4.4 | 是（四角色） |
| Plans（无价格） | §4.6 | 是（无货币字段） |
| Subscriptions | §4.7 | 是（Active/Suspended） |
| UsageMeters | §4.8 | 是（deviceCount/telemetryPoints/period） |
| QuotaPolicies | §4.9 | 是（ThresholdPct/Action） |
| Devices / FaultEvents / Telemetry* | §4.10 / §4.11 / §4.12 | 是（含 TenantId） |
| OutboxMessages | §4.13 | 是 |
| AuditLogs | §4.14 | 是（tenant_id 可空） |
| （增量）RefreshTokens | §4.5 | 新增（§5.2 refresh 所需，未列于 §6 但为登录刷新前置） |
