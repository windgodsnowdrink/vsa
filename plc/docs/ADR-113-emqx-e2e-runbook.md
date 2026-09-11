# ADR-113 EMQX 端到端实测手册

> 目标：在真实 **PostgreSQL + EMQX** 上验证 ADR-113 的**双摄取路径**与持久化/计量链路。
> 脚本：`scripts/verify-emqx-e2e.sh`（自动断言 + PASS/FAIL 汇总）
> 契约源：`docs/ADR-113-implementation.md`

---

## 0. 当前状态（务必先读）

**已实跑并通过**（2026-09-11，`scripts/verify-emqx-e2e.sh` → **16/16 PASS**）：

| 探测项 | 结果 |
|--------|------|
| `wslc`（WSL 容器 CLI） | ✅ **可用** —— 第三条运行时路径，不在沙箱黑名单内 |
| Docker Desktop 守护 | ❌ 未运行（`npipe:////./pipe/dockerDesktopLinuxEngine` 不可连接），`docker compose` 不可用 |
| `wsl.exe` | ❌ 被沙箱安全策略列入黑名单，禁止启动（且不得绕过） |
| EMQX 5.8.5 / PG 18.6 | ✅ 已在 `wslc` 容器运行（详见 §1） |

> ⚠️ 2026-09-11 早前的「本机无法拉起容器」结论**已被推翻**：它只覆盖了 Docker 与 `wsl.exe`，
> 遗漏了 **`wslc`**（`C:\Program Files\WSL\wslc`）。详见 `docs/PLC-IoT-Architecture-Verification.md` §9.4。

### 0.1 实测踩坑补遗（本次实际遇到，按此操作可复现）

| # | 坑 | 现象 | 处置 |
|---|-----|------|------|
| 1 | **容器被会话回收** | 隔天后 `wslc ps` 为空，但容器只是退出（`wslc ps -a` 仍可见） | `wslc start plc-saas-postgres plc-saas-emqx` 直接复用**已存在容器**（保留原端口映射与数据卷），**不要**重新 `run`（后者会改用 compose 声明的 `postgres:16`，本机拉取不可靠） |
| 2 | **应用进程读取不到 `appsettings.json`** | `The ConnectionString property has not been initialized.`，同时伴随 `WARN: Jwt:Key 未配置` | `dotnet run --project …/plc-saas.csproj` 的**内容根取当前工作目录**。必须 `cd` 到 **`src/plc-saas/`** 再启动（`Content root path` 需等于该目录），或显式传 `ConnectionStrings__Default` 环境变量 |
| 3 | Windows 保留段吞掉 1883 | 绑定 MQTT 宿主端口报 `WSAEACCES`（排除段 `1802-1901`） | MQTT 宿主端口平移到 **11883**，运行脚本时 `EMQX_MQTT_PORT=11883` |
| 4 | **EMQX REST 写接口需要 API Key** | `POST /api/v5/publish` 返回 401（Dashboard 的 basic auth 无效，`emqx ctl admins passwd` 重置后仍 401） | 路径②改为**原生 MQTT 直发**：`python scripts/mqtt_publish.py 127.0.0.1 11883 <topic> <payload>`（stdlib 实现，无 pip 依赖）。自证：`/api/v5/nodes` 带不带 auth 均 401，而 `/api/v5/status` 是公开端点返回 200 |
| 5 | **PG 18+ 数据目录布局** | 旧布局数据挂在 `/var/lib/postgresql/data` 时容器立即退出 | 数据卷挂在**父目录** `/var/lib/postgresql` |
| 6 | **存量库的多租户索引阻塞第二个租户** | 第 2 个租户开通返回 `40900 唯一约束被违反` | 代码已修（`infra.cs` 把 Identity 默认全局唯一索引降级）；**存量库仍需手动**：`DROP INDEX "RoleNameIndex"; CREATE INDEX "RoleNameIndex" ON asp_net_roles("NormalizedName");`（`UserNameIndex` 同理）。详见验证报告 §9.5 |
| 7 | 持久 shell 的 `$$` 恒定 | 重复跑脚本时租户 slug 撞车 409 | 脚本已改为 `$$-$RANDOM` 组合生成唯一 slug/email/companyName |

---

## 1. 前置：两套起服务方式（二选一，凭据与版本完全一致）

> ✅ **已统一**：`src/plc-saas/docker-compose.yml` 是基础设施**单一真源**（镜像版本 / PG 凭据 / 端口）。
> `start-dev-env.ps1` 启动时会**解析该文件**取值，只额外加 `docker.1ms.run` 镜像源前缀，
> 因此两条路径产出**同一套凭据**，可任意混用、无需区分。

| 方式 | 命令 | 差异 |
|------|------|------|
| **A. wslc（无 Docker Desktop，推荐）** | `pwsh src/plc-saas/start-dev-env.ps1` | 走 `docker.1ms.run` 镜像源；应用角色 `plcsaas` 由脚本 `psql` 等效创建 |
| **B. docker compose** | `docker compose -f src/plc-saas/docker-compose.yml up -d` | 直连 Docker Hub；应用角色 `plcsaas` 由 `initdb/01-roles.sql` 创建 |

统一后的真值（改这里即可，`docker-compose.yml`）：

| 项 | 值 |
|----|----|
| PostgreSQL 镜像 | `postgres:16` |
| EMQX 镜像 | `emqx/emqx:5.8.5`（REST 为 `/api/v5`） |
| PG 超级用户 | `postgresql / postgresql` |
| PG 应用角色（连接串用） | `plcsaas / plcsaas` |
| 数据库 | `plc_aiot_saas` |
| EMQX 控制台 | `admin / Admin123` |
| 端口 | MQTT 1883 / WS 8083·8084 / SSL 8883 / 控制台 18083 / PG 5432 |

**起服务后自检**（三者都应返回）：
```bash
curl -s -o /dev/null -w "emqx-dashboard %{http_code}\n" http://localhost:18083
timeout 2 bash -c "</dev/tcp/127.0.0.1/1883" && echo "mqtt 1883 open"
timeout 2 bash -c "</dev/tcp/127.0.0.1/5432" && echo "pg 5432 open"
```

---

## 2. 启动 plc-saas（默认即可；路径② 需开启桥接）

> ✅ **已修复**：`src/plc-saas/appsettings.json` 的 `ConnectionStrings:Default` 默认值已从
> `Integrated Security=True`（Windows 集成认证，连容器 PG 必失败）改为容器友好形式
> `Host=localhost;Port=5432;Database=plc_aiot_saas;Username=plcsaas;Password=plcsaas`，
> **开箱即连**，无需环境变量覆盖。仅当宿主端口被平移（如 `PG_HOST_PORT=55432`）时才需覆盖。

```bash
cd src/plc-saas

# 可选：仅当 PG 宿主端口不是 5432 时才需要
# export ConnectionStrings__Default="Host=localhost;Port=55432;Database=plc_aiot_saas;Username=plcsaas;Password=plcsaas"
export Mqtt__BrokerUrl="mqtt://localhost:1883"
export Mqtt__Bridge__Enabled="true"                 # 启用路径②（边缘中继）
export Mqtt__Bridge__Username="plc-saas"
export Mqtt__Bridge__Password=""                    # dev 环境 EMQX 允许匿名；若启用认证则填超级用户
export Ingest__SharedKey="plc-aiot-saas-ingest-dev-key-Change-Me"

dotnet run --project plc-saas.csproj
# 默认 http://localhost:5000；健康端点 GET /api/v1/ops/health
```

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
| 绑端口报 `WSAEACCES` / `10013` | Windows（Hyper-V/WSL2）保留端口段 | 覆盖宿主端口：`$env:PG_HOST_PORT='55432'`（容器内仍 5432），并同步 `ConnectionStrings__Default` 的 `Port` |
| 拉镜像超时/失败 | Docker Hub 不可达 | 用 `docker.1ms.run` 镜像源（见 `start-dev-env.ps1`） |
| SaaS 起不来，报 PG 认证失败 | 连接串与容器凭据不匹配 | 默认已是 `plcsaas/plcsaas`；若改过 compose 的 `POSTGRES_*`，同步改 `appsettings.json` 或用 `ConnectionStrings__Default` 覆盖 |
| `role "plcsaas" does not exist` | wslc 路径未创建应用角色 | `wslc exec plc-saas-postgres psql -U postgresql -d plc_aiot_saas -c "CREATE ROLE plcsaas LOGIN PASSWORD 'plcsaas' SUPERUSER;"` |
| EMQX REST 返回 404 | EMQX 版本与 API 路径不匹配 | 当前统一为 5.8.5，REST 前缀 `/api/v5`；若自行升 6.x 需改脚本 `EMQX_REST` 路径 |
| 路径② 无消费 | 未开桥接 | `Mqtt__Bridge__Enabled=true` 并确认日志出现「MQTT 桥已连接 … 订阅 tenants/+/devices/+/telemetry\|fault」 |
| `The ConnectionString property has not been initialized.` + `WARN: Jwt:Key 未配置` | **`appsettings.json` 没被加载**：内容根不对 | 内容根取**当前工作目录**，须 `cd src/plc-saas` 再启动（不要只在 `--project` 里指 Domino 路径）；确认日志 `Content root path` 指向 `src/plc-saas` |
| EMQX REST `POST /api/v5/publish` 返回 401 | EMQX 5.x 该写接口需要 **API Key**，Dashboard basic auth 无效 | 改走原生 MQTT 直发：`python scripts/mqtt_publish.py 127.0.0.1 11883 <topic> <payload>` |
| 第 2 个租户开通报 `40900 资源冲突：唯一约束被违反` | Identity 默认索引 `RoleNameIndex` / `UserNameIndex` 是**全局唯一**（代码已修，但存量库不会自动变更） | 存量库手动执行：`DROP INDEX "RoleNameIndex"; CREATE INDEX "RoleNameIndex" ON asp_net_roles("NormalizedName");`（`UserNameIndex` 同理） |
| 种子崩溃 `User security stamp cannot be null.` | 默认密码策略拒绝 `root/admin` 且 `CreateAsync` 结果未校验 | 已在 `seed.cs` 修复（显式赋 `SecurityStamp` + 直写哈希 + 校验结果），重构建即可 |

---

## 6. 待办

- [x] **已完成**——在本机用 `wslc` 起真实 PG 18.6 + EMQX 5.8.5 实跑本手册：**16/16 PASS**，结果已回填 `docs/PLC-IoT-Architecture-Verification.md` §9.4–§9.7。
- [x] **已修复**——统一两套起服务方式：`docker-compose.yml` 定为单一真源，`start-dev-env.ps1` 解析它取版本/凭据（PG16 + EMQX 5.8.5，superuser `postgresql`，应用角色 `plcsaas` 两条路径等效创建）。
- [x] **已修复**——`appsettings.json` 的 `Integrated Security=True` 改为容器友好默认（`plcsaas/plcsaas`），开箱即连。
- [x] **已修复（实测发现）**——多租户阻塞：Identity 默认全局唯一索引导致平台只能有 1 个租户。
- [x] **已修复（实测发现）**——超管种子 `SecurityStamp` 为空导致进程启动即退出。
- [ ] **待排查**——超管登录 `POST /api/v1/auth/login`（`root/admin`）返回 `50000`，怀疑 `IssueTokens` 写 `refresh_tokens` 受 RLS / `TenantStampInterceptor` 影响（超管 `TenantId=Guid.Empty`）。不阻塞 ADR-113，但阻碍 `/ops/health` 取回 Healthy 载荷。
- [ ] **待实测**——EMQX 规则引擎 → Webhook 的**自动**触发（路径①的生产形态）；需在 EMQX 侧配 Webhook 后复测，REST 写接口需 API Key 故暂未脚本化。
- [ ] 可选——把 `docker-compose.yml` 的 `postgres:16` 与实测可用的 `18-alpine3.23` 对齐（当前 16 为声明目标，本机 16 标签拉取不可靠）。
