#!/usr/bin/env bash
# ADR-113 端到端实测：EMQX + PostgreSQL 双摄取路径验证
#
# 覆盖：
#   路径①（主）  HTTP 摄取  POST /api/v1/ingest/telemetry | /api/v1/ingest/fault
#   路径②（边缘）MQTTnet 订阅桥，经 EMQX 发布到 tenants/+/devices/+/telemetry|fault
#   持久化断言  PG: devices / fault_events / outbox_messages / usage_meters
#   负向用例    错误 Ingest-Key→401、未知租户→404
#
# 前置（满足其一）：
#   A) wslc（项目约定）： pwsh src/plc-saas/start-dev-env.ps1
#   B) docker compose   ： docker compose -f src/plc-saas/docker-compose.yml up -d
# 说明：本脚本**不负责拉起**容器（不同环境运行时差异大），只做检测与验证；
#       未检测到 PG/EMQX 时会明确报错退出，便于先按 runbook 起服务。
#
# 用法：
#   bash scripts/verify-emqx-e2e.sh
#   SAAS_URL=http://localhost:5000 TENANT_ID=<guid> bash scripts/verify-emqx-e2e.sh
#
# 相关文档：docs/ADR-113-implementation.md、docs/ADR-113-emqx-e2e-runbook.md

set -uo pipefail
cd "$(dirname "$0")/.." || exit 1

# ---------------- 配置（可用环境变量覆盖） ----------------
SAAS_URL="${SAAS_URL:-http://localhost:5000}"
API="${SAAS_URL}/api/v1"
INGEST_KEY="${INGEST_KEY:-plc-aiot-saas-ingest-dev-key-Change-Me}"
EMQX_REST="${EMQX_REST:-http://localhost:18083}"
EMQX_REST_PORT="${EMQX_REST_PORT:-18083}"
# Windows（Hyper-V/WSL2）会保留部分 TCP 段（本机 1802-1901 被占，1883 无法绑定），
# 起容器时若把 MQTT 宿主端口平移到 11883，用 EMQX_MQTT_PORT 覆盖即可。
EMQX_MQTT_PORT="${EMQX_MQTT_PORT:-1883}"
EMQX_USER="${EMQX_USER:-admin}"
EMQX_PASS="${EMQX_PASS:-Admin123}"
PG_HOST_PORT="${PG_HOST_PORT:-5432}"
PG_USER="${PG_USER:-postgresql}"
PG_DB="${PG_DB:-plc_aiot_saas}"
TENANT_ID="${TENANT_ID:-}"
DEVICE_ID="${DEVICE_ID:-}"

PASS=0; FAIL=0
chk() { # chk <名称> <0|1>
  if [ "$2" -eq 0 ]; then echo "[e2e] PASS $1"; PASS=$((PASS+1));
  else echo "[e2e] FAIL $1"; FAIL=$((FAIL+1)); fi
}
has() { grep -qi "$2" <<<"$1" && return 0 || return 1; }

echo "== ADR-113 EMQX 端到端实测 =="
echo "SaaS: $SAAS_URL   EMQX REST: $EMQX_REST   EMQX MQTT: localhost:$EMQX_MQTT_PORT   PG: localhost:$PG_HOST_PORT/$PG_DB"
echo

# ---------------- 0. 基础设施可达性 ----------------
echo "== 0) 基础设施可达性 =="
port_open() { timeout 2 bash -c "</dev/tcp/127.0.0.1/$1" 2>/dev/null; }
port_open "$EMQX_MQTT_PORT" && chk "emqx-mqtt-$EMQX_MQTT_PORT" 0 || chk "emqx-mqtt-$EMQX_MQTT_PORT" 1
port_open "$EMQX_REST_PORT" && chk "emqx-rest-$EMQX_REST_PORT" 0 || chk "emqx-rest-$EMQX_REST_PORT" 1
port_open "$PG_HOST_PORT" && chk "postgres-$PG_HOST_PORT" 0 || chk "postgres-$PG_HOST_PORT" 1

if ! port_open "$EMQX_MQTT_PORT" || ! port_open "$PG_HOST_PORT"; then
  echo
  echo "!! EMQX / PostgreSQL 未就绪，无法继续实测。"
  echo "   请先按 docs/ADR-113-emqx-e2e-runbook.md 起服务："
  echo "     A) pwsh src/plc-saas/start-dev-env.ps1        （wslc）"
  echo "     B) docker compose -f src/plc-saas/docker-compose.yml up -d"
  echo "   提示：Windows 可能保留了 1883 所在端口段（WSAEACCES），起容器时把 MQTT"
  echo "         宿主端口平移到 11883，然后以 EMQX_MQTT_PORT=11883 运行本脚本。"
  echo "== 结果: $FAIL 项失败（基础设施未就绪）=="
  exit 1
fi

# ---------------- 1. SaaS 健康 ----------------
echo
echo "== 1) SaaS 健康 /ops/health =="
# 契约说明：/ops/health 显式 RequireAuthorization("SuperAdminOnly")（slices/ops.health.cs），
# 匿名访问返回 401 + 空 body 属于**预期行为**，同时证明进程存活且鉴权未旁路。
# 要取回 Healthy 载荷必须带超管 Bearer token（先 POST /api/v1/auth/login）。
HCODE=$(curl -s -o /tmp/e2e_health.json -w "%{http_code}" -m 10 "$API/ops/health" 2>/dev/null)
HEALTH=$(cat /tmp/e2e_health.json 2>/dev/null)
case "$HCODE" in
  200|401) chk "saas-health-responds($HCODE)" 0;;
  *)       chk "saas-health-responds($HCODE)" 1;;
esac
echo "      health HTTP $HCODE: ${HEALTH:0:160}"

# ---------------- 2. 准备租户 ----------------
echo
echo "== 2) 准备租户 =="
if [ -z "$TENANT_ID" ]; then
  # 契约对齐 tenant.register.cs -> TenantCreateRequest(CompanyName, Slug, AdminEmail, AdminPassword)
  # 注意字段是 companyName（不是 name）；slug 需满足 ^[a-z0-9][a-z0-9-]{2,30}$（本处以 pid 结尾，合法）
  # slug/email 必须每次运行唯一：同一持久 shell 里 $$ 恒定不变，
  # 复用旧 pid 会撞上历史租户返回 409「子域已被占用」（tenant.register.cs 幂等以 slug 唯一为准）。
  RUN_TAG="$$-$RANDOM"
  REG=$(curl -s -m 15 -X POST "$API/tenants" -H "Content-Type: application/json" \
        -d "{\"companyName\":\"e2e-emqx-tenant-$RUN_TAG\",\"slug\":\"e2e-emqx-$RUN_TAG\",\"adminEmail\":\"e2e$RUN_TAG@example.com\",\"adminPassword\":\"Pass@12345\"}")
  TENANT_ID=$(sed -n 's/.*"id"\s*:\s*"\([0-9a-fA-F-]\{36\}\)".*/\1/p' <<<"$REG" | head -1)
  [ -n "$TENANT_ID" ] && chk "tenant-created" 0 || chk "tenant-created" 1
  # 失败时把服务端报文打出来，避免只剩一句 FAIL 无从定位
  [ -n "$TENANT_ID" ] || echo "      tenant 创建响应: ${REG:0:300}"
else
  chk "tenant-provided" 0
fi
echo "      tenantId: ${TENANT_ID:-<空>}"
if [ -z "$TENANT_ID" ]; then
  echo "!! 无租户 Id，无法继续（可通过 TENANT_ID=<guid> 指定）。"
  echo "== 结果: $FAIL 项失败 =="; exit 1
fi

[ -n "$DEVICE_ID" ] || DEVICE_ID=$(python -c "import uuid;print(uuid.uuid4())" 2>/dev/null || cat /proc/sys/kernel/random/uuid 2>/dev/null)
echo "      deviceId: $DEVICE_ID"

# ---------------- 3. 路径① HTTP 摄取（主路径） ----------------
echo
echo "== 3) 路径① HTTP 摄取 =="
TS=$(date +%s)
TCODE=$(curl -s -o /tmp/ing_t.json -w "%{http_code}" -m 15 -X POST "$API/ingest/telemetry" \
  -H "Content-Type: application/json" -H "X-Tenant-Id: $TENANT_ID" -H "X-Ingest-Key: $INGEST_KEY" \
  -d "{\"deviceId\":\"$DEVICE_ID\",\"ts\":$TS,\"metrics\":{\"temp\":23.5,\"rpm\":1200}}")
echo "      telemetry -> HTTP $TCODE  body: $(head -c 120 /tmp/ing_t.json 2>/dev/null)"
case "$TCODE" in 200|201|202) chk "ingest-telemetry" 0;; *) chk "ingest-telemetry" 1;; esac

FCODE=$(curl -s -o /tmp/ing_f.json -w "%{http_code}" -m 15 -X POST "$API/ingest/fault" \
  -H "Content-Type: application/json" -H "X-Tenant-Id: $TENANT_ID" -H "X-Ingest-Key: $INGEST_KEY" \
  -d "{\"deviceId\":\"$DEVICE_ID\",\"code\":\"E001\",\"severity\":\"high\",\"message\":\"过温 e2e\"}")
echo "      fault     -> HTTP $FCODE  body: $(head -c 120 /tmp/ing_f.json 2>/dev/null)"
case "$FCODE" in 200|201|202) chk "ingest-fault" 0;; *) chk "ingest-fault" 1;; esac

# ---------------- 4. 负向用例 ----------------
echo
echo "== 4) 负向用例（鉴权/隔离）=="
BAD=$(curl -s -o /dev/null -w "%{http_code}" -m 10 -X POST "$API/ingest/telemetry" \
  -H "Content-Type: application/json" -H "X-Tenant-Id: $TENANT_ID" -H "X-Ingest-Key: wrong-key" \
  -d "{\"deviceId\":\"$DEVICE_ID\",\"metrics\":{\"temp\":1}}")
echo "      错误 Ingest-Key -> HTTP $BAD"
[ "$BAD" = "401" ] && chk "bad-ingest-key-401" 0 || chk "bad-ingest-key-401" 1

# 未知租户必须用**合法但不存在的随机 Guid**。Guid.Empty 走的是鉴权分支：
# ingest.http.cs: IngestAuth.Authenticate 对 Guid.Empty 返回 403「缺少或非法的 X-Tenant-Id」，
# 与「租户不存在 404」是两个不同的负向用例，不能混用。
UNKNOWN_T=$(python -c "import uuid;print(uuid.uuid4())" 2>/dev/null || cat /proc/sys/kernel/random/uuid 2>/dev/null)
UNK=$(curl -s -o /dev/null -w "%{http_code}" -m 10 -X POST "$API/ingest/telemetry" \
  -H "Content-Type: application/json" -H "X-Tenant-Id: $UNKNOWN_T" -H "X-Ingest-Key: $INGEST_KEY" \
  -d "{\"deviceId\":\"$DEVICE_ID\",\"metrics\":{\"temp\":1}}")
echo "      未知租户(random) -> HTTP $UNK"
[ "$UNK" = "404" ] && chk "unknown-tenant-404" 0 || chk "unknown-tenant-404" 1

EMPTY=$(curl -s -o /dev/null -w "%{http_code}" -m 10 -X POST "$API/ingest/telemetry" \
  -H "Content-Type: application/json" -H "X-Tenant-Id: 00000000-0000-0000-0000-000000000000" -H "X-Ingest-Key: $INGEST_KEY" \
  -d "{\"deviceId\":\"$DEVICE_ID\",\"metrics\":{\"temp\":1}}")
echo "      空租户Guid.Empty -> HTTP $EMPTY"
[ "$EMPTY" = "403" ] && chk "empty-tenant-guid-403" 0 || chk "empty-tenant-guid-403" 1

# ---------------- 5. 路径② MQTT 订阅桥 ----------------
echo
echo "== 5) 路径② MQTTnet 订阅桥（需 Mqtt:Bridge:Enabled=true 启动 SaaS）=="
# 为什么走 MQTT 直发而不是 EMQX REST /api/v5/publish：
#   EMQX 5.x 的 REST 写接口需要 **API Key**，Dashboard 的 basic auth 对它们一律 401
#   （可自证：/api/v5/nodes 带不带 auth 都 401，而 /api/v5/status 是公开端点返回 200）。
#   直发 MQTT 既绕开该鉴权，也更贴近路径②真实链路（Edge → Broker → 订阅桥）。
# 这里刻意用**另一个全新设备**：它从未走路径①，若最终落到 devices 表，即可确证是被桥消费的。
PUB_SCRIPT="$(dirname "$0")/mqtt_publish.py"
MQ_DEVICE_ID=$(python -c "import uuid;print(uuid.uuid4())" 2>/dev/null || cat /proc/sys/kernel/random/uuid 2>/dev/null)
MQ_TOPIC="tenants/$TENANT_ID/devices/$MQ_DEVICE_ID/telemetry"
MQ_PAYLOAD="{\"ts\":$TS,\"metrics\":{\"temp\":42.0,\"rpm\":1500},\"device_id\":\"$MQ_DEVICE_ID\"}"
PUB_OUT=$(python "$PUB_SCRIPT" 127.0.0.1 "$EMQX_MQTT_PORT" "$MQ_TOPIC" "$MQ_PAYLOAD" 2>&1)
echo "      mqtt publish -> $PUB_OUT"
case "$PUB_OUT" in MQTT-PUBLISH-OK*) chk "mqtt-publish" 0;; *) chk "mqtt-publish" 1;; esac

# 给桥接消费留出时间（异步写入 devices / outbox）
sleep 6
echo "      桥接消费断言见第 6 步：devices 表中应出现全新的 $MQ_DEVICE_ID"

# ---------------- 6. PG 持久化断言 ----------------
echo
echo "== 6) PG 持久化断言 =="
# 自动识别运行时：wslc 容器 或 docker 容器
run_psql() {
  local sql="$1"
  if command -v wslc >/dev/null 2>&1 && wslc ps 2>/dev/null | grep -q 'plc-saas-postgres'; then
    wslc exec plc-saas-postgres psql -U "$PG_USER" -d "$PG_DB" -tAc "$sql" 2>/dev/null
  elif command -v docker >/dev/null 2>&1 && docker ps >/dev/null 2>&1; then
    docker exec plc-saas-postgres psql -U "$PG_USER" -d "$PG_DB" -tAc "$sql" 2>/dev/null
  else
    return 127
  fi
}

DEV_CNT=$(run_psql "select count(*) from devices where \"Id\"='$DEVICE_ID';")
if [ -n "$DEV_CNT" ] && [ "$DEV_CNT" -ge 1 ] 2>/dev/null; then chk "pg-device-autoprovisioned" 0; else chk "pg-device-autoprovisioned" 1; fi
echo "      devices(该设备): ${DEV_CNT:-<查询失败>}"

FLT_CNT=$(run_psql "select count(*) from fault_events where \"DeviceId\"='$DEVICE_ID';")
if [ -n "$FLT_CNT" ] && [ "$FLT_CNT" -ge 1 ] 2>/dev/null; then chk "pg-fault-event" 0; else chk "pg-fault-event" 1; fi
echo "      fault_events: ${FLT_CNT:-<查询失败>}"

OB_CNT=$(run_psql "select count(*) from outbox_messages;")
if [ -n "$OB_CNT" ] && [ "$OB_CNT" -ge 1 ] 2>/dev/null; then chk "pg-outbox-written" 0; else chk "pg-outbox-written" 1; fi
echo "      outbox_messages(总): ${OB_CNT:-<查询失败>}"

# PG 列名是 PascalCase，未加引号会被 PostgreSQL 折叠成小写而报列不存在，必须加双引号。
UM=$(run_psql "select coalesce(sum(\"TelemetryPoints\"),0) from usage_meters;")
echo "      usage_meters.\"TelemetryPoints\"(总): ${UM:-<查询失败>}"
[ -n "$UM" ] && chk "pg-metering-readable" 0 || chk "pg-metering-readable" 1

# 路径②专属证据：该设备只经 MQTT 发布（从未走 HTTP 摄取），出现在 devices 表即证明订阅桥生效。
MQ_CNT=$(run_psql "select count(*) from devices where \"Id\"='$MQ_DEVICE_ID';")
if [ -n "$MQ_CNT" ] && [ "$MQ_CNT" -ge 1 ] 2>/dev/null; then chk "pg-mqtt-bridge-consumed" 0; else chk "pg-mqtt-bridge-consumed" 1; fi
echo "      devices(仅经MQTT的设备): ${MQ_CNT:-<查询失败>}"

# ---------------- 结果 ----------------
echo
echo "== 结果: $FAIL 项失败 =="
echo "   通过 $PASS 项。租户=$TENANT_ID 设备=$DEVICE_ID"
[ "$FAIL" -eq 0 ] && exit 0 || exit 1
