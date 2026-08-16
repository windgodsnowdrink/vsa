# ADR-113 实现说明：真实 MQTT/EMQX 摄取桥（V1-now 已落地）

> 状态：**Accepted（V1-now 已实现）** · 决策源：`docs/decisions/ADR-109-113.md#ADR-113`
> 优先级：⭐ P0（数据平面关键路径） · 代码：`plc-saas/slices/ingest.bridge.cs`、`plc-saas/slices/ingest.http.cs`

---

## 1. 目标

把 `MQTT.cs` 空壳升级为真实数据平面桥：EMQX 上的设备遥测/故障消息，经两条等价路径汇聚到 `plc-saas`，
写入时序库/关系库并实时推送前端。SaaS 自此才有实时数据（ADR-113 背景）。

## 2. 双摄取路径（统一收敛到 `Ingest*Command`）

```
                  ┌──────────────────────────┐
 设备 ──MQTT──▶ EMQX ─┬─ ① 规则引擎 ─HTTP─▶ POST /api/v1/ingest/telemetry  ─┐
                      │                      POST /api/v1/ingest/fault      │
                      │                                                      ▼
                      └─ ② MqttBridgeService（MQTTnet 订阅）──▶ IngestTelemetryCommand ─┐
                                                                   IngestFaultCommand ──┤
                                                                                          ▼
                                           ┌──────────────┬──────────────┬──────────────────┐
                                           │ Device 映射  │ Outbox 计量   │ SignalR /hubs/faults│
                                           │(自动纳管)    │(TelemetryIng.)│ (FaultRaised 直推) │
                                           └──────────────┴──────────────┴──────────────────┘
                                                │                │                    │
                                          PG devices      PG outbox_messages  前端角标(tenant group)
                                          PG fault_events   │(计量代理消费)         │
                                                          ▼                    │
                                                  usage_meters.telemetry_points ◀┘
```

- **① 主路径（默认启用）**：EMQX 规则引擎把设备主题转发到 `plc-saas` 的 HTTPS 摄取端点。无状态、易水平扩展，是 ADR-113 首选。
- **② 边缘中继（可选，`Mqtt:Bridge:Enabled=true`）**：`plc-saas` 以 EMQX superuser 身份用 **MQTTnet** 直接订阅 `tenants/+/devices/+/telemetry` 与 `.../fault`，从主题解析 `tid+deviceId` 后复用同一命令。适合未配置规则引擎或需边缘聚合的场景。两条路径逻辑完全一致，零重复。

## 3. 摄取契约（HTTP 主路径）

### 遥测 `POST /api/v1/ingest/telemetry`
- **Headers**：`X-Tenant-Id: <tid(guid)>`、`X-Ingest-Key: <shared secret>`
- **Body**：
  ```json
  { "deviceId": "<guid>", "ts": 1690000000, "metrics": { "temp": 23.5, "rpm": 1200 } }
  ```
  - `deviceId` 可省，改由 `topic` 字段兜底解析（`tenants/<tid>/devices/<deviceId>/telemetry`）。
  - `ts` 为 unix 秒，缺省取 UTC 现在。

### 故障 `POST /api/v1/ingest/fault`
- **Headers**：同上。
- **Body**：
  ```json
  { "deviceId": "<guid>", "code": "E001", "severity": "high", "message": "过温" }
  ```

### 鉴权与隔离
- 端点 `.AllowAnonymous()`（server-to-server，非用户 JWT）；以 `X-Ingest-Key` 比对 `Ingest:SharedKey`，以 `X-Tenant-Id` 比对合法 Guid。
- 未配置 `Ingest:SharedKey` → `503`；密钥错 → `401`；租户不存在 → `404`；租户暂停 → `409`（AC-07）。
- 写操作显式 `ICurrentTenant.TenantId = tid`（匿名请求无 JWT，连接级 `app.tenant_id` 经 `TenantConnectionInterceptor` 设置，RLS 兜底）；跨租户设备 Id → `403`。

## 4. 设备映射
- `deviceId` 为全局唯一 Guid（`Device.Id`）。
- 首条遥测/故障自动纳管设备：`Device` 不存在则创建（Name=Id，Status=online/fault），已存在则校验 TenantId 一致，否则 `403` 跨租户。

## 5. 写入与实时推送
- **遥测**：`ITelemetryStore.WriteAsync` → 配置 `Telemetry:Influx:Url` 时写 InfluxDB v2（行协议，best-effort，不可达仅告警、不阻断摄取）；否则 `NullTelemetryStore` 兜底。同时入 `OutboxMessages(Type=TelemetryIngested, payload 含 points)`，由 `MeteringAgent` 累加 `usage_meters.telemetry_points`（ADR-102/108，与存储解耦）。
- **故障**：`FaultEvent` 入 PG（Status=open）；发布 `FaultRaised` 通知 → `FaultRaisedPushHandler` 经 `IHubContext<FaultsHub>` 推送到租户 SignalR 组（前端角标，同 ADR-105 `FaultAcked` 范式）。设备 Status 置 `fault`。

## 6. 配置（appsettings.json）
```jsonc
"Mqtt": {
  "BrokerUrl": "mqtt://localhost:1883",
  "Bridge": { "Enabled": false, "Username": "plc-saas", "Password": "", "ClientId": "plc-saas-bridge" }
},
"Ingest": { "SharedKey": "plc-aiot-saas-ingest-dev-key-Change-Me" },
"Telemetry": { "Influx": { "Url": "", "Bucket": "plc_telemetry", "Org": "plc-aiot", "Token": "" } }
```
- `Mqtt:Bridge:Enabled=true` 才启用 MQTTnet 订阅桥；Broker 不可达时退避重试，不崩溃宿主。
- `Telemetry:Influx:Url` 留空则用 Null 兜底（仍走计量 Outbox）。

## 7. EMQX 规则引擎配置（主路径）
在 EMQX Dashboard → 集成 → 规则，建 SQL（遥测示例，故障同理改 topic/字段）：
```sql
SELECT
  username        AS tenant_id,
  payload.device_id AS device_id,
  payload.ts      AS ts,
  payload.metrics AS metrics,
  topic           AS topic
FROM "tenants/+/devices/+/telemetry"
```
动作选 **Webhook / HTTP Sink**：
- URL：`http://<plc-saas>:5000/api/v1/ingest/telemetry`
- 方法：`POST`
- 请求头：`X-Tenant-Id: ${tenant_id}`、`X-Ingest-Key: <shared key>`
- 主体模板：
  ```json
  { "deviceId": "${device_id}", "ts": ${ts}, "metrics": ${metrics} }
  ```
> `username` 即设备连接 EMQX 时用的 MQTT-JWT 的 `tid`（见 `slices/identity.mqtt.cs` / ADR-107）。EMQX JWT 认证 + ACL 保证只有合法租户主题可达规则引擎。

## 8. 本地联调（与 start-dev-env.ps1 配合）
1. `pwsh plc-saas/start-dev-env.ps1` 起 PG + EMQX（wslc）。
2. `dotnet run --project plc-saas/plc-saas.csproj` 启动 SaaS（默认 `http://localhost:5000`）。
3. 用 MQTTX 连 `mqtt://localhost:1883`，username=某租户 tid（须先在 EMQX 配 JWT 认证或 dev 超级用户），发布：
   - 主题 `tenants/<tid>/devices/<deviceGuid>/telemetry`，载荷 `{"ts":<unix>,"metrics":{"temp":23.5}}`
   - 主题 `tenants/<tid>/devices/<deviceGuid>/fault`，载荷 `{"code":"E001","severity":"high"}`
4. 主路径验证：直接 `curl -X POST localhost:5000/api/v1/ingest/telemetry -H "X-Tenant-Id: <tid>" -H "X-Ingest-Key: <key>" -d '{"deviceId":"<guid>","metrics":{"temp":1}}'`。
5. 前端订阅 `/hubs/faults`（租户组），故障产生时应收到 `fault.raised` 推送。

## 9. 测试门禁
- `plc-saas.verify` 全量编译所有切片（含桥接）+ ADR-108 红线静态扫描（无 price/currency/amount/money）+ 切片存在性断言。
- 新增桥接逻辑建议补单测：`DeviceResolver` 自动纳管、遥测 Outbox 写入 `points`、故障创建并发 `FaultRaised`。

## 10. 后续（P1/P2）
- 引入 Wolverine Outbox 替代轮询 `MeteringAgent`（ADR-110，接通真实 EMQX 传输后收益明显）。
- 遥测也可直写 PG 时序表作为 InfluxDB 不可用时的强一致兜底（当前仅计量走 PG）。
