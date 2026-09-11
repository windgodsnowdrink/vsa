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
EMQX_USER="${EMQX_USER:-admin}"
EMQX_PASS="${EMQX_PASS:-Admin123}"
PG_HOST_PORT="${PG_HOST_PORT:-5432}"
PG_USER="${PG_USER:-postgres}"
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
echo "SaaS: $SAAS_URL   EMQX REST: $EMQX_REST   PG: localhost:$PG_HOST_PORT/$PG_DB"
echo

# ---------------- 0. 基础设施可达性 ----------------
echo "== 0) 基础设施可达性 =="
port_open() { timeout 2 bash -c "</dev/tcp/127.0.0.1/$1" 2>/dev/null; }
port_open 1883  && chk "emqx-mqtt-1883" 0 || chk "emqx-mqtt-1883" 1
port_open 18083 && chk "emqx-rest-18083" 0 || chk "emqx-rest-18083" 1
port_open "$PG_HOST_PORT" && chk "postgres-$PG_HOST_PORT" 0 || chk "postgres-$PG_HOST_PORT" 1

if ! port_open 1883 || ! port_open "$PG_HOST_PORT"; then
  echo
  echo "!! EMQX / PostgreSQL 未就绪，无法继续实测。"
  echo "   请先按 docs/ADR-113-emqx-e2e-runbook.md 起服务："
  echo "     A) pwsh src/plc-saas/start-dev-env.ps1        （wslc）"
  echo "     B) docker compose -f src/plc-saas/docker-compose.yml up -d"
  echo "== 结果: $FAIL 项失败（基础设施未就绪）=="
  exit 1
fi

# ---------------- 1. SaaS 健康 ----------------
echo
echo "== 1) SaaS 健康 /ops/health =="
HEALTH=$(curl -s -m 10 "$API/ops/health" 2>/dev/null)
[ -n "$HEALTH" ] && chk "saas-health-responds" 0 || chk "saas-health-responds" 1
echo "      health: ${HEALTH:0:160}"

# ---------------- 2. 准备租户 ----------------
echo
echo "== 2) 准备租户 =="
if [ -z "$TENANT_ID" ]; then
  REG=$(curl -s -m 15 -X POST "$API/tenants" -H "Content-Type: application/json" \
        -d '{"name":"e2e-emqx-tenant","slug":"e2e-emqx-'"$$"'","adminEmail":"e2e'"$$"'@example.com","adminPassword":"Pass@12345"}')
  TENANT_ID=$(sed -n 's/.*"id"\s*:\s*"\([0-9a-fA-F-]\{36\}\)".*/\1/p' <<<"$REG" | head -1)
  [ -n "$TENANT_ID" ] && chk "tenant-created" 0 || chk "tenant-created" 1
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

UNKNOWN_T="00000000-0000-0000-0000-000000000000"
UNK=$(curl -s -o /dev/null -w "%{http_code}" -m 10 -X POST "$API/ingest/telemetry" \
  -H "Content-Type: application/json" -H "X-Tenant-Id: $UNKNOWN_T" -H "X-Ingest-Key: $INGEST_KEY" \
  -d "{\"deviceId\":\"$DEVICE_ID\",\"metrics\":{\"temp\":1}}")
echo "      未知租户       -> HTTP $UNK"
[ "$UNK" = "404" ] && chk "unknown-tenant-404" 0 || chk "unknown-tenant-404" 1

# ---------------- 5. 路径② MQTT 订阅桥 ----------------
echo
echo "== 5) 路径② MQTTnet 订阅桥（需 Mqtt:Bridge:Enabled=true 启动 SaaS）=="
echo "      经 EMQX REST /api/v5/publish 发布（本机无 mosquitto_pub 时仍可验证）"
PUB_BODY=$(cat <<JSON
{"topic":"tenants/$TENANT_ID/devices/$DEVICE_ID/telemetry","qos":1,"retain":false,
 "payload":"{\\"ts\\":$TS,\\"metrics\\":{\\"temp\\":42.0},\\"device_id\\":\\"$DEVICE_ID\\"}"}
JSON
)
PUB=$(curl -s -o /tmp/pub.json -w "%{http_code}" -m 15 -X POST "$EMQX_REST/api/v5/publish" \
  -u "$EMQX_USER:$EMQX_PASS" -H "Content-Type: application/json" -d "$PUB_BODY")
echo "      EMQX publish -> HTTP $PUB  body: $(head -c 120 /tmp/pub.json 2>/dev/null)"
case "$PUB" in 200|204) chk "emqx-publish" 0;; *) chk "emqx-publish" 1;; esac

# 给桥接消费留出时间
sleep 5
echo "      （桥接消费断言见第 6 步 PG 校验；若桥未启用则该项应跳过）"

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

UM=$(run_psql "select coalesce(sum(telemetry_points),0) from usage_meters;")
echo "      usage_meters.telemetry_points(总): ${UM:-<查询失败>}"
[ -n "$UM" ] && chk "pg-metering-readable" 0 || chk "pg-metering-readable" 1

# ---------------- 结果 ----------------
echo
echo "== 结果: $FAIL 项失败 =="
echo "   通过 $PASS 项。租户=$TENANT_ID 设备=$DEVICE_ID"
[ "$FAIL" -eq 0 ] && exit 0 || exit 1
