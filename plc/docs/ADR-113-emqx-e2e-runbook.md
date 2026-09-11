# ADR-113 EMQX 端到端实测手册

> 目标：在真实 **PostgreSQL + EMQX** 上验证 ADR-113 的**双摄取路径**与持久化/计量链路。
> 脚本：`scripts/verify-emqx-e2e.sh`（自动断言 + PASS/FAIL 汇总）
> 契约源：`docs/ADR-113-implementation.md`

---

## 0. 当前状态（务必先读）

**本仓库所在环境尚未实跑**（2026-09-11 探测结论）：

| 探测项 | 结果 |
|--------|------|
| `wslc` / `wsl` / `docker` / `docker-compose` | 均已安装 |
| Docker Desktop 守护 | **未运行**（`npipe:////./pipe/dockerDesktopLinuxEngine` 不可连接） |
| `wsl.exe` | **被沙箱安全策略列入黑名单**，禁止启动 |
| 端口 1883 / 18083 / 5432 | 全部未开放（服务未起） |
| MQTTnet | 已引用（`plc-saas.csproj:37`，v5.2.0.1603） |

结论：**本机无法拉起容器，实测需在具备 Docker 守护（或可用 WSL）的机器上执行**。脚本与手册已按真实契约写好，可直接运行。

---

## 1. 前置：两套起服务方式（二选一）

> ⚠️ 两种方式**镜像与凭据不同**，不要混用：

| 方式 | 命令 | 镜像 | PG 凭据 | EMQX |
|------|------|------|---------|------|
| **A. wslc（项目约定，推荐）** | `pwsh src/plc-saas/start-dev-env.ps1` | postgres:18-alpine3.23、emqx **6.1.4** | `postgres / postgres` | admin / Admin123 |
| **B. docker compose** | `docker compose -f src/plc-saas/docker-compose.yml up -d` | postgres:16、emqx **5.8.5** | superuser `postgresql`；应用角色 `plcsaas / plcsaas` | admin / Admin123 |

- 方式 A 经 `docker.1ms.run` 镜像源拉取（**Docker Hub 在本机不可达**）。
- 方式 B 的 `initdb/01-roles.sql` 会创建应用角色 `plcsaas`。
- 端口：MQTT 1883 / WS 8083·8084 / SSL 8883 / 控制台 18083；PG 5432。

**起服务后自检**（三者都应返回）：
```bash
curl -s -o /dev/null -w "emqx-dashboard %{http_code}\n" http://localhost:18083
timeout 2 bash -c "</dev/tcp/127.0.0.1/1883" && echo "mqtt 1883 open"
timeout 2 bash -c "</dev/tcp/127.0.0.1/5432" && echo "pg 5432 open"
```

---

## 2. 启动 plc-saas（关键：必须覆盖连接串与开启桥接）

`src/plc-saas/appsettings.json` 默认 `Integrated Security=True`（Windows 集成认证），**连容器 PG 会失败**，必须用环境变量覆盖；路径② 还需显式开启 MQTTnet 订阅桥。

```bash
cd src/plc-saas

export ConnectionStrings__Default="Host=localhost;Port=5432;Database=plc_aiot_saas;Username=postgres;Password=postgres"
export Mqtt__BrokerUrl="mqtt://localhost:1883"
export Mqtt__Bridge__Enabled="true"                 # 启用路径②（边缘中继）
export Mqtt__Bridge__Username="plc-saas"
export Mqtt__Bridge__Password=""                    # dev 环境 EMQX 允许匿名；若启用认证则填超级用户
export Ingest__SharedKey="plc-aiot-saas-ingest-dev-key-Change-Me"

dotnet run --project plc-saas.csproj
# 默认 http://localhost:5000；健康端点 GET /api/v1/ops/health
```

> 方式 B（compose）时用 `Username=plcsaas;Password=plcsaas`。

---

## 3. 执行实测脚本

```bash
bash scripts/verify-emqx-e2e.sh
```

可覆盖变量：`SAAS_URL`、`INGEST_KEY`、`EMQX_REST`/`EMQX_USER`/`EMQX_PASS`、`PG_HOST_PORT`/`PG_USER`/`PG_DB`、`TENANT_ID`、`DEVICE_ID`。

### 断言清单

| # | 断言 | 期望 | 说明 |
|---|------|------|------|
| 0 | `emqx-mqtt-1883` / `emqx-rest-18083` / `postgres-5432` | 端口开放 | 未就绪则脚本直接退出并提示起服务 |
| 1 | `saas-health-responds` | `/api/v1/ops/health` 有响应 | — |
| 2 | `tenant-created` | 从 `POST /api/v1/tenants` 解析到 Guid | 或用 `TENANT_ID=` 指定 |
| 3 | `ingest-telemetry` / `ingest-fault` | 200/201/202 | **路径①主路径** |
| 4 | `bad-ingest-key-401` | 401 | 错误 `X-Ingest-Key` |
| 4 | `unknown-tenant-404` | 404 | 未知 `X-Tenant-Id` |
| 5 | `emqx-publish` | 200/204 | 经 EMQX REST `/api/v5/publish` 发布（本机无 `mosquitto_pub` 亦可验证） |
| 6 | `pg-device-autoprovisioned` | ≥1 | 首条遥测自动纳管设备 |
| 6 | `pg-fault-event` | ≥1 | `fault_events` 落库 |
| 6 | `pg-outbox-written` | ≥1 | 计量 Outbox 写入 |
| 6 | `pg-metering-readable` | 可读 | `usage_meters.telemetry_points` |

> 说明：第 5/6 步对**路径②**的强断言依赖 `Mqtt__Bridge__Enabled=true`；未开启时 `emqx-publish` 仍会通过（消息进了 EMQX），但不会体现为 SaaS 侧消费。

---

## 4. 手工验证（可选，MQTTX）

1. MQTTX 连 `mqtt://localhost:1883`（dev 可匿名；启用认证时 username = 租户 tid）。
2. 发布遥测：
   - 主题 `tenants/<tid>/devices/<deviceGuid>/telemetry`
   - 载荷 `{"ts":<unix秒>,"metrics":{"temp":23.5}}`
3. 发布故障：
   - 主题 `tenants/<tid>/devices/<deviceGuid>/fault`
   - 载荷 `{"code":"E001","severity":"high"}`
4. 前端订阅 SignalR `/hubs/faults`（租户组），故障应收到 `fault.raised` 推送。

### 路径① 的 EMQX 规则引擎（生产主路径）

Dashboard → 集成 → 规则，SQL：
```sql
SELECT username AS tenant_id, payload.device_id AS device_id,
       payload.ts AS ts, payload.metrics AS metrics, topic AS topic
FROM "tenants/+/devices/+/telemetry"
```
动作选 **Webhook / HTTP Sink**：
- URL：`http://<plc-saas>:5000/api/v1/ingest/telemetry`，方法 `POST`
- 头：`X-Tenant-Id: ${tenant_id}`、`X-Ingest-Key: <shared key>`
- 体：`{"deviceId":"${device_id}","ts":${ts},"metrics":${metrics}}`

---

## 5. 故障排查

| 现象 | 原因 | 处理 |
|------|------|------|
| `failed to connect to the docker API at npipe://...` | Docker Desktop 守护未启动 | 启动 Docker Desktop，或改用 `wslc` |
| `wsl.exe: Permission denied` / 被安全策略拦截 | `wsl.exe` 在程序黑名单 | 需在安全中心 → 命令安全 → 程序黑名单中移除；**不要尝试绕过** |
| 绑端口报 `WSAEACCES` / `10013` | Windows（Hyper-V/WSL2）保留端口段 | 覆盖宿主端口：方式 A 用 `PG_HOST_PORT=55432`（容器内仍 5432） |
| 拉镜像超时/失败 | Docker Hub 不可达 | 用 `docker.1ms.run` 镜像源（见 `start-dev-env.ps1`） |
| SaaS 起不来，报 PG 认证失败 | 连接串用了 `Integrated Security=True` | 用 `ConnectionStrings__Default` 环境变量覆盖为用户名/密码形式 |
| EMQX REST 返回 404 | EMQX 6.x 与 5.x 的 API 路径有差异 | 确认镜像版本（方式 A=6.1.4，方式 B=5.8.5），按需调整 `/api/v5` |
| 路径② 无消费 | 未开桥接 | `Mqtt__Bridge__Enabled=true` 并确认日志出现「MQTT 桥已连接 … 订阅 tenants/+/devices/+/telemetry\|fault」 |

---

## 6. 待办

- [ ] 在具备 Docker 守护的机器上实跑本手册，回填 `docs/PLC-IoT-Architecture-Verification.md` 的真实结果。
- [ ] 统一两套起服务方式（镜像版本 + PG 凭据）为单一来源，消除 A/B 差异。
- [ ] `appsettings.json` 的 `Integrated Security=True` 建议改为容器友好的默认（或用 `.Development.json` 区分）。
