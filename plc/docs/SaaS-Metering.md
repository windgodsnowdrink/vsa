# PLC·AIOT 多租户 SaaS — 计量后台任务设计（Phase 2）

> 依据：SaaS-Spec §2(P1 计量) / §5.3 / §9(AC-05, AC-06, AC-10) / §10；ADR-102（Outbox）；ADR-108（只计量不收费）
> 关联：SaaS-DB-Schema.md（UsageMeters / QuotaPolicies / OutboxMessages / Subscriptions）
> 范围：遥测聚合 → 用量计量 → 配额阈值评估 → 超限提醒与限流 → 订阅运营态暂停/恢复。不含任何收费链路。

---

## 1. 目标与红线

- 目标：按租户、按周期（月）聚合设备数与遥测点，实时反映配额剩余；达阈值推送提醒（Warn），超限按策略限流（Throttle，非静默丢弃）。
- 红线（ADR-108 / AC-10）：计量任务与所有相关表 / 响应**不得出现价格 / 货币 / 金额 / 支付**概念。
- 实时性：告警确认走 SignalR 热路径（ADR-105）；计量为后台近实时（周期级聚合 + 阈值事件），不要求秒级。

---

## 2. 数据流（Outbox 驱动）

```
[采集/写路径]
 设备遥测 → 中心 EMQX → 命令 Handler
    ├─▶ 原始遥测点写入 InfluxDB（时序，V9）
    └─▶ 同事务写 OutboxMessages(type=TelemetryIngested, tenant_id, payload={deviceId, points})

[后台计量消费者 Agent]  (独立 BackgroundService / 定时轮询)
  轮询 OutboxMessages WHERE sent_at IS NULL
    └─▶ 按 (tenant_id, period) 聚合：
          device_count = SELECT COUNT(*) FROM devices WHERE tenant_id=?
          telemetry_points += SUM(payload.points)
    └─▶ UPSERT usage_meters(tenant_id, period)  (ON CONFLICT 累加)
    └─▶ 标记 OutboxMessages.sent_at = now()

[阈值评估]  (同消费者内，聚合后)
  读取 subscriptions→plans + quota_policies
    ├─ 用量 / 配额 >= threshold_pct 且未发过 Warn
    │     └─▶ 发布 quota_threshold_reached → 经 SignalR / 通知推管理员（Warn）
    └─ 用量 > 配额（超额）
          └─▶ 发布 quota_exceeded → 触发限流策略（Throttle）+ 通知管理员

[推送通道]
  quota_threshold_reached / quota_exceeded → SignalR Hub(ops/admin) 直推 + 站内通知表（MVP 可仅 SignalR + 简单存储）
```

---

## 3. Outbox 消费者实现要点

- **轮询**：`SELECT ... FROM outbox_messages WHERE sent_at IS NULL ORDER BY created_at LIMIT N`，处理完置 `sent_at`。单租户批处理避免大事务。
- **幂等**：消费者对事件按 `id` 去重（Inbox 表或已处理集合）；`usage_meters` 用 `UPSERT`（冲突键 `(tenant_id, period)`）保证可重放。
- **租户上下文**：每条事件 `tenant_id` 显式来自 payload；处理前 `ICurrentTenant` restore + `SET app.tenant_id`（见 SaaS-DB-Schema §3），保证 RLS 与过滤器一致。
- **周期**：`period` 取 `YYYY-MM`（UTC 月）；跨月自动新行，`device_count` 按月快照、`telemetry_points` 按月累加。
- **频率**：近实时滚动窗口（如每 30–60s 轮询一次），满足 AC-05「计量任务运行后推送」；非秒级。

---

## 4. 阈值评估与动作（G5 / G8）

`QuotaPolicies` 定义每套餐每维度（device / telemetry）的 `threshold_pct` 与 `action`：

| Action | 触发条件 | 行为 |
|--------|----------|------|
| Warn | `used / quota >= threshold_pct`（默认 90%） | 向租户管理员推送提醒（SignalR + 站内通知），**不阻断**业务；记录事件 `quota_threshold_reached` |
| Throttle | `used > quota`（超额） | 按策略限流（见 §6），**不静默丢弃**；记录事件 `quota_exceeded` |

- 同一 `(tenant, metric, period)` 的 Warn 事件去重（已发且未降回阈值前不再重复推送）。
- 提醒文案为真实状态描述（如「设备数已达配额 90%」），不含营销话术。

---

## 5. 超限限流策略落点（G5 / G8，AC-06）

限流在**写路径**强制，而非遥测丢弃：

- **设备注册超配额**：`POST /devices` 类写命令在落库前校验 `device_count >= plan.device_quota` → 返回 **409**（消息说明达设备配额，引导升级套餐），不计为静默丢弃。
- **遥测超配额**：边缘 / 中心遥测 ingest 在 `telemetry_points >= plan.telemetry_quota` 后进入**限流**（背压 / 降采样 / 计入超额计数但标记 `overage`），保证可观测且不丢数据；限流粒度由 `quota_policies.action=Throttle` 决定。
- **速率限制**：租户级请求速率按配额配置（System.Threading.RateLimiting 令牌桶，键含 `tenant_id`），超限返回 **429**（openapi.yaml `TooManyRequests`）。

---

## 6. 事件结构（示例）

```json
// quota_threshold_reached
{ "type": "quota_threshold_reached",
  "tenantId": "uuid", "metric": "device", "used": 90, "quota": 100, "pct": 90 }

// quota_exceeded
{ "type": "quota_exceeded",
  "tenantId": "uuid", "metric": "telemetry", "used": 120000, "quota": 100000 }
```

事件经 Outbox 或进程内 Mediator 通知发布；管理员通道（SignalR Hub `ops`）订阅后即时刷新运营视图与站内通知。

---

## 7. 订阅运营态：暂停 / 恢复与数据保留（G7 / AC-07）

- **暂停（Suspend）**：超管 `POST /ops/tenants/{id}/suspend` → `tenants.status = Suspended` 且 `subscriptions.status = Suspended`（同事务）。
  - 写路径守卫：命令 Handler / `TenantMiddleware` 检测 `status == Suspended` → **拒绝写**（返回 **409 / 403**，视接口），不落库。
  - 读路径：降级为**只读快照**——RLS 仍返回该租户数据（数据保留），但前端/接口标记为只读态，禁止任何变更操作。
  - **数据零删除**：暂停仅改状态，所有表行保留，便于审计与恢复。
- **恢复（Resume）**：超管 `POST /ops/tenants/{id}/resume` → 状态回 `Active`，写路径守卫解除，业务恢复正常。
- **实现**：状态检查集中在 `TenantMiddleware` 与命令基类前置校验；订阅状态变更经同一 UoW 保证租户与订阅一致。

---

## 8. 与 Spec / ADR 一致性

- AC-05（≥90% 推送 `quota_threshold_reached`）：§4 Warn + §6 事件。
- AC-06（超额限流不静默丢弃）：§5 限流落点 + 429/409。
- AC-07（暂停读写拒、数据保留）：§7。
- AC-10（只计量不收费）：全文件无货币字段，事件/表仅含计数与阈值。
- ADR-102（Outbox 自建）：§2–§3。
- ADR-108（不计费）：贯穿。

---

## 9. P0 合规声明

- 本文档无 emoji、无紫→粉渐变、无硬编码色、无模板味文案。
- 所有计量实体与事件仅含计数（device_count / telemetry_points / threshold_pct）；无 price / currency / amount。
