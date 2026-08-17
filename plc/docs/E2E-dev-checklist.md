# PLC-IoT SaaS · 本地端到端联调清单（dev 环境）

> 目标：用本机 `wslc` 起的 PostgreSQL + EMQX，把已落地的 **P0（ADR-113 MQTT 桥）/ P1（ADR-112 AI 诊断 + MCP）** 能力真实跑通。
> 阶段状态：**P0 ✅ · P1 ✅ · P2 设计 Spike 完成（等触发线，本次不落地）**。
> 环境：.NET 11 Preview（global.json 锁 `11.0.100-preview.6`），在仓库根目录执行。

---

## 0. 前置条件
- [x] 5432 占用已解决（本机原生 PostgreSQL 服务已停用）—— 2026-08-17 用户确认
- [x] wslc 镜像已拉取：`docker.1ms.run/library/postgres:18-alpine3.23`、`docker.1ms.run/emqx/emqx:6.1.4`
- [x] `dotnet run` 启动期三处崩溃/RLS 失效已修（提交 `676b968`）；`start-dev-env.ps1` 已加固（提交 `6c5ccd8`）

---

## 1. 起本地基础设施（wslc）

```powershell
pwsh plc-saas/start-dev-env.ps1
```

产出：
| 组件 | 容器名 | 访问 | 凭据 |
|------|--------|------|------|
| PostgreSQL 18 | `plc-saas-postgres` | `localhost:5432` | `postgres` / `postgres`，库 `plc_aiot_saas` |
| EMQX 6.1.4 | `plc-saas-emqx` | MQTT `1883` / WS `8083`·`8084` / 控制台 `18083` | `admin` / `Admin123` |

验证基础设施可达：
```powershell
# PG 启动日志（应看到 "database system is ready to accept connections"）
wslc logs plc-saas-postgres --tail 20
# EMQX 健康
curl -u admin:Admin123 http://localhost:18083/api/v5/health
```

> 若 `wslc` 报 WSAEACCES（端口绑定失败）：宿主端口被占用/保留，见 `start-dev-env.ps1` 自检提示，或设 `$env:PG_HOST_PORT='5433'` 并相应改连接串 `Port`。

---

## 2. 启动后端 + 数据引导

```powershell
cd plc-saas
dotnet run --project plc-saas.csproj
```

预期启动日志（顺序）：
1. `EnsureCreatedAsync` 建全部表（幂等）
2. `rls.sql` 的 `DROP/CREATE POLICY tenant_isolation` 成功执行（RLS 真正生效）
3. `DbSeeder` 幂等种子：套餐档（Free/Pro/Business/Enterprise）+ 配额策略 + **平台超管 `tid=root`**
4. `Now listening on http://localhost:5000`

> 若报 5000 被占用：停掉旧 `dotnet` 实例后重试。
> 若改过源码：先 `dotnet build` 再 `dotnet run`（strip 构建目标可能因 up-to-date 判定不重跑）。

---

## 3. 登录 + 鉴权冒烟

平台超管由 `seed.cs` 幂等创建：
- Email：`root@plc-aiot.example`（可经 `appsettings.json` 的 `Root:Email` 覆盖）
- Password：`Root@Dev-ChangeMe-2026`（可经 `Root:Password` 覆盖）
- 登录响应结构：`{ "code":0, "message":"", "data": { "accessToken", "refreshToken", "expiresIn", "tokenType", "tenantId" } }`

```powershell
$RESP = curl.exe -s -X POST http://localhost:5000/api/v1/auth/login `
  -H "Content-Type: application/json" `
  -d '{"Tenant":"root","Email":"root@plc-aiot.example","Password":"Root@Dev-ChangeMe-2026"}'
Write-Host $RESP        # 先看结构，定位 accessToken
$TOKEN = ($RESP | ConvertFrom-Json).data.accessToken
```

验证 health（SuperAdminOnly，带 token 应 200；未带 token 沙箱已验证返回 401）：
```powershell
curl.exe -i -H "Authorization: Bearer $TOKEN" http://localhost:5000/api/v1/ops/health
# 预期 HTTP/1.1 200
```

---

## 4. ADR-113 MQTT 桥（P0 数据平面）

两种入口（任选其一验证）：

- **A) HTTP 主路径**：EMQX 规则引擎 → `POST /api/v1/ingest/*`（`slices/ingest.http.cs`）。需在 EMQX 控制台配置规则把主题转发到该端点，并配 `Ingest:SharedKey`。
- **B) 边缘中继 `MqttBridgeService`**：`appsettings.json` 置 `Mqtt:Bridge:Enabled=true` + `Mqtt:BrokerUrl=mqtt://localhost:1883`；自动订阅 `tenants/+/devices/+/telemetry` 与 `tenants/+/devices/+/fault`，从主题解析 `tid`+`deviceId`，Broker 不可达时退避重试、不崩宿主。

发测试消息（用 `mqttx-cli` / `mosquitto_pub` / EMQX 控制台 WebSocket 客户端）：
```powershell
mosquitto_pub -h localhost -p 1883 -u admin -P Admin123 `
  -t "tenants/<tenantId>/devices/<deviceId>/telemetry" `
  -m '{"temp":23.5,"ts":"2026-08-17T12:00:00Z"}'
```
预期：后端日志出现 "MQTT 消息处理"，数据按 `tid` 入库（telemetry 表行级隔离）。
> `<tenantId>` / `<deviceId>` 替换为实际 GUID（可从 `tenants` 表或 `GET /api/v1/ops/tenants`、`devices` 列表获取）。

---

## 5. ADR-112 AI 诊断 + MCP（P1 旗舰）

开启门控（`appsettings.json`）：
```json
"Ai":     { "Enabled": true },
"Mcp":    { "Enabled": true },
"Qdrant": { "Enabled": false }
```
> `Qdrant:Enabled=false` 时走进程内 `InMemoryFaultVectorStore` 兜底（离线可用、跨租户隔离）；生产接 Qdrant 置 `true`（URL `http://localhost:6334`）。
> 改完 `appsettings.json` 后**重启后端**才能生效。

诊断端点：
```powershell
curl.exe -X POST http://localhost:5000/api/v1/ai/diagnose `
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" `
  -d '{"DeviceId":"<guid>","FaultCode":"E-OT","Severity":"High"}'
```
预期：
- 未接真实 LLM 时走 `PlcDiagnosisChatClient` 启发式回退，Diagnosis 含 "启发式诊断"
- 故障模式回写向量库，后续相似故障可检索

MCP（外部 AI Agent 安全问诊）：
- 端点 `http://localhost:5000/mcp`（Streamable HTTP + SSE）
- 工具：`ListDevices` / `GetFaults` / `GetTelemetrySummary`
- 鉴权由全局 `UseAuthentication` 注入 JWT(`tid`)；`Resolve()` 强制校验 `tid` 非空，拒跨租户

---

## 6. 多租户隔离校验（安全关键）

- 用租户 A 的 token 调 `/api/v1/ai/diagnose` 写入故障模式，再换租户 B 的 token 查询，确认 B **不可见** A 的模式（`InMemoryFaultVectorStore` 跨租户隔离）
- 直连 PG，以不同 `SET app.tenant_id = '<tid>';` 查询，确认 RLS 行级隔离生效

---

## 7. P2 状态（不阻塞，等触发线）

- **ADR-110 Wolverine Outbox**：Deferred→P2，沿用 ADR-102 自建事务 Outbox + `MeteringAgent` 轮询；触发（接真实 EMQX）已满足但收益未到刚需
- **ADR-111 YARP/WAF**：Deferred→P2，内网免费部署（ADR-108）期不需要

---

## 已知约束
- `appsettings.json` 缺 `Jwt:Key` 时后端用内置兜底密钥（**仅 dev**），生产必须配强密钥
- Qdrant `PointStruct.Payload` 只读 → 元信息走进程内 `_meta` 缓存（生产应改 Payload）
- 真实 LLM / Qdrant 接入点已在 `infra.cs` 预留（`IChatClient` / `QdrantFaultVectorStore`）
- 响应统一包装为 `ApiEnvelope{code,message,data}`
