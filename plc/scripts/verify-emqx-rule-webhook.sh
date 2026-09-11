#!/usr/bin/env bash
# ADR-113 路径①「生产形态」端到端验证：EMQX 规则引擎 → HTTP Webhook → plc-saas 摄取
#
# 与 scripts/verify-emqx-e2e.sh 的区别（两条脚本验证的不是同一件事，别混用）：
#   verify-emqx-e2e.sh        路径①是**直打** /api/v1/ingest/*（模拟 Webhook 的结果），
#                             路径②是 .NET 侧 MQTTnet 订阅桥。
#   本脚本                    路径①的**真实链路**：由 EMQX 规则引擎匹配 topic、渲染 body、
#                             主动 HTTP POST 到 plc-saas。规则引擎是否配置正确、能否触发，
#                             只有本脚本能证明。
#
# 实验设计（关键：排除路径②干扰）
#   本验证要求 SaaS **不要**启用 .NET 订阅桥（appsettings.json 默认 Mqtt:Bridge:Enabled=false）。
#   桥接关闭时，一条只经 MQTT 发布的遥测若仍落到 devices 表，唯一解释就是被 EMQX 规则引擎
#   转发到了 /api/v1/ingest/telemetry —— 证据排他，不存在路径②的混淆。
#
# 前置
#   1) 容器已起：PG(5432) + EMQX(11883/18083)，见 docs/ADR-113-emqx-e2e-runbook.md
#   2) SaaS 需监听 0.0.0.0（容器要能访问宿主；只绑 localhost 时容器连不上）：
#        cd src/plc-saas && ASPNETCORE_URLS=http://0.0.0.0:5000 dotnet bin/Debug/net11.0/plc-saas.dll
#   3) 本机 curl 多为 Windows 原生版，不认 /tmp 路径，故统一用 stdin 传 body（--data-binary @-）
#   4) 环境存在 http_proxy 会劫持本机请求，脚本内部统一 --noproxy '*'
#
# 用法
#   bash scripts/verify-emqx-rule-webhook.sh
#   EMQX_MQTT_PORT=11883 SAAS_HOST_IP=172.25.64.1 bash scripts/verify-emqx-rule-webhook.sh

set -uo pipefail
cd "$(dirname "$0")/.." || exit 1

# ---------------- 配置（可用环境变量覆盖） ----------------
SAAS_URL="${SAAS_URL:-http://127.0.0.1:5000}"
API="${SAAS_URL}/api/v1"
SAAS_PORT="${SAAS_PORT:-5000}"
INGEST_KEY="${INGEST_KEY:-plc-aiot-saas-ingest-dev-key-Change-Me}"

EMQX_REST="${EMQX_REST:-http://127.0.0.1:18083}"
EMQX_REST_PORT="${EMQX_REST_PORT:-18083}"
# 宿主 1883 常落在 Windows 保留 TCP 段（1802-1901），容器多映射到 11883
EMQX_MQTT_PORT="${EMQX_MQTT_PORT:-11883}"

PG_USER="${PG_USER:-postgresql}"
PG_DB="${PG_DB:-plc_aiot_saas}"

# EMQX REST 只认 API Key，Dashboard 的 basic auth 对它一律 401。
# 第一把 Key 只能靠 bootstrap 文件造（/api/v5/api_key 自己也拒 API Key 自管理）。
API_KEY_ID="${API_KEY_ID:-plc-e2e-admin}"
BOOTSTRAP_FILE="${BOOTSTRAP_FILE:-/opt/emqx/data/bootstrap_api_key}"

CONTAINER="${CONTAINER:-plc-saas-emqx}"
RULE_ID="${RULE_ID:-plc_telemetry_to_saas}"
ACTION_NAME="${ACTION_NAME:-plc_saas_telemetry_webhook}"
CONNECTOR_NAME="${CONNECTOR_NAME:-plc_saas_http}"

PASS=0; FAIL=0
chk() { if [ "$2" = "0" ]; then echo "[rule] PASS $1"; PASS=$((PASS+1)); else echo "[rule] FAIL $1"; FAIL=$((FAIL+1)); fi; }

echo "== ADR-113 路径①真实链路：EMQX 规则引擎 -> Webhook -> plc-saas =="
echo "SaaS: $SAAS_URL   EMQX REST: $EMQX_REST   EMQX MQTT: localhost:$EMQX_MQTT_PORT"

# ---------------- 0) 容器运行时识别 ----------------
run_emqx() { # 在 EMQX 容器内执行
  if command -v wslc >/dev/null 2>&1 && wslc list 2>/dev/null | grep -q "$CONTAINER"; then
    wslc exec "$CONTAINER" sh -c "$1"
  elif command -v docker >/dev/null 2>&1 && docker ps >/dev/null 2>&1; then
    docker exec "$CONTAINER" sh -c "$1"
  else
    return 127
  fi
}
run_psql() {
  if command -v wslc >/dev/null 2>&1 && wslc list 2>/dev/null | grep -q 'plc-saas-postgres'; then
    wslc exec plc-saas-postgres psql -U "$PG_USER" -d "$PG_DB" -tAc "$1" 2>/dev/null
  elif command -v docker >/dev/null 2>&1 && docker ps >/dev/null 2>&1; then
    docker exec plc-saas-postgres psql -U "$PG_USER" -d "$PG_DB" -tAc "$1" 2>/dev/null
  else
    return 127
  fi
}

echo
echo "== 0) 基础设施可达性 =="
# nc 在本机不存在；用 python socket 探测（脚本已依赖 python）
python -c "import socket;s=socket.create_connection(('127.0.0.1',int('$EMQX_MQTT_PORT')),3);s.close()" 2>/dev/null
chk "emqx-mqtt-$EMQX_MQTT_PORT" $?
# 判定用 HTTP 状态码而非 curl 退出码：Windows 版 curl 带 -o /dev/null 会返回 23（写错误），误判为不可达
REST_CODE=$(curl -s -o /dev/null -w "%{http_code}" -m 8 --noproxy '*' "$EMQX_REST/api/v5/status")
echo "      /api/v5/status -> HTTP $REST_CODE"
[ "$REST_CODE" = "200" ] && chk "emqx-rest-$EMQX_REST_PORT" 0 || chk "emqx-rest-$EMQX_REST_PORT" 1
run_psql "select 1;" >/dev/null 2>&1; chk "postgres-reachable" $?

# ---------------- 1) 造/取 EMQX API Key ----------------
echo
echo "== 1) EMQX API Key（bootstrap 文件方式）=="
API_SECRET="$(run_emqx "cut -d: -f2 $BOOTSTRAP_FILE 2>/dev/null | tr -d '\r\n'")"
if [ -z "$API_SECRET" ]; then
  API_SECRET="$(head -c 24 /dev/urandom | od -An -tx1 | tr -d ' \n')"
  run_emqx "printf '%s:%s\n' '$API_KEY_ID' '$API_SECRET' > $BOOTSTRAP_FILE" >/dev/null 2>&1
  run_emqx "printf 'api_key = { bootstrap_file = \"$BOOTSTRAP_FILE\" }\n' > /tmp/apikey.hocon && emqx ctl conf load --merge /tmp/apikey.hocon" >/dev/null 2>&1
  echo "      已写入 bootstrap 文件并热加载配置"
else
  echo "      复用已有 bootstrap key"
fi
AUTH=(-u "$API_KEY_ID:$API_SECRET")

# /api/v5/nodes 是**需鉴权**端点：Dashboard 的 admin/Admin123 对它 401，API Key 才 200。
# 注意别用 /api/v5/status 判断鉴权——它是公开端点，不带鉴权也 200。
AUTH_CODE=$(curl -s -o /dev/null -w "%{http_code}" -m 10 --noproxy '*' "${AUTH[@]}" "$EMQX_REST/api/v5/nodes")
echo "      /api/v5/nodes -> HTTP $AUTH_CODE"
[ "$AUTH_CODE" = "200" ] && chk "emqx-api-key-auth" 0 || chk "emqx-api-key-auth" 1
if [ "$AUTH_CODE" != "200" ]; then
  echo "!! API Key 不可用，无法创建规则引擎资源。"; echo "== 结果: $FAIL 项失败 =="; exit 1
fi

# ---------------- 2) 探测「容器可访问的宿主地址」 ----------------
# 容器访问宿主：局域网 IP 与 docker 网关(172.17.0.1)通常不通，
# Hyper-V 默认交换机(如 172.25.64.1)才通。逐个试，取第一个能拿到 HTTP 应答的。
echo
echo "== 2) 容器 -> 宿主 SaaS 连通性 =="
CANDIDATES="${SAAS_HOST_IP:-$(ipconfig 2>/dev/null | grep -aoE '172\.25\.[0-9]+\.[0-9]+' | head -1) host.docker.internal 172.25.64.1 172.17.0.1}"
HOST_IP=""
for c in $CANDIDATES; do
  R=$(run_emqx "curl -s -o /dev/null -w '%{http_code}' -m 5 --noproxy '*' http://$c:$SAAS_PORT/api/v1/ops/health" 2>/dev/null | tail -1)
  if [ "$R" = "200" ] || [ "$R" = "401" ]; then HOST_IP="$c"; echo "      $c -> HTTP $R  (200/401 均表示可达)"; break; fi
  echo "      $c -> HTTP ${R:-000} (不可达)"
done
[ -n "$HOST_IP" ] && chk "container-to-saas-reachable" 0 || chk "container-to-saas-reachable" 1
if [ -z "$HOST_IP" ]; then
  echo "!! 容器访问不到宿主 SaaS。请确认 SaaS 以 0.0.0.0 启动（只绑 localhost 时容器连不上）："
  echo "     cd src/plc-saas && ASPNETCORE_URLS=http://0.0.0.0:$SAAS_PORT dotnet bin/Debug/net11.0/plc-saas.dll"
  echo "   或用 SAAS_HOST_IP=<容器可访问的宿主IP> 显式指定。"
  echo "== 结果: $FAIL 项失败 =="; exit 1
fi

# ---------------- 3) 准备租户 ----------------
echo
echo "== 3) 准备租户 =="
RUN_TAG="$$-$RANDOM"
REG=$(curl -s -m 15 --noproxy '*' -X POST "$API/tenants" -H "Content-Type: application/json" \
  -d "{\"companyName\":\"rule-webhook-$RUN_TAG\",\"slug\":\"rule-webhook-$RUN_TAG\",\"adminEmail\":\"rw$RUN_TAG@example.com\",\"adminPassword\":\"Pass@12345\"}")
TENANT_ID=$(sed -n 's/.*"id"\s*:\s*"\([0-9a-fA-F-]\{36\}\)".*/\1/p' <<<"$REG" | head -1)
[ -n "$TENANT_ID" ] && chk "tenant-created" 0 || chk "tenant-created" 1
echo "      tenantId: ${TENANT_ID:-<空>}"
[ -n "$TENANT_ID" ] || { echo "      $REG"; echo "== 结果: $FAIL 项失败 =="; exit 1; }

# ---------------- 4) 建 connector / action / rule（幂等：先删后建） ----------------
echo
echo "== 4) 创建规则引擎资源 =="
post_json() { # post_json <path> <json>
  printf '%s' "$2" | curl -s -m 25 --noproxy '*' "${AUTH[@]}" -X POST "$EMQX_REST/api/v5/$1" \
    -H "Content-Type: application/json" --data-binary @- -w "\n%{http_code}"
}
del_res() { curl -s -o /dev/null -m 15 --noproxy '*' "${AUTH[@]}" -X DELETE "$EMQX_REST/api/v5/$1"; }

# 规则引用 action，action 引用 connector，删除要反过来
# 注意：action/connector 的地址是 **id**（http:<name>），不是 name。
# 用 name 删除会 404 静默失败 -> 下次 POST 报 ALREADY_EXISTS -> 沿用旧配置（例如旧的 X-Tenant-Id 头），
# 导致数据被写进上一个租户，且现象极具迷惑性（设备照样建出来）。
del_res "rules/$RULE_ID" 2>/dev/null
del_res "actions/http:$ACTION_NAME" 2>/dev/null
del_res "connectors/http:$CONNECTOR_NAME" 2>/dev/null

C_OUT=$(post_json "connectors" "{\"type\":\"http\",\"name\":\"$CONNECTOR_NAME\",\"url\":\"http://$HOST_IP:$SAAS_PORT/api/v1/ingest/telemetry\",\"enable\":true,\"description\":\"plc-saas ingest endpoint\"}")
C_CODE=$(tail -1 <<<"$C_OUT")
echo "      connector -> HTTP $C_CODE"
[ "$C_CODE" = "201" ] || [ "$C_CODE" = "200" ] && chk "connector-created" 0 || chk "connector-created" 1
[ "$C_CODE" = "201" ] || [ "$C_CODE" = "200" ] || echo "      $(head -c 300 <<<"$C_OUT")"

# method 必须是小写枚举(post/get/put/delete)，写 "POST" 会 400 not_a_enum_symbol。
# body 里的 ${deviceId}/${ts}/${temp} 由规则 SQL 的输出字段渲染。
ACTION_JSON='{"type":"http","name":"'"$ACTION_NAME"'","connector":"'"$CONNECTOR_NAME"'","enable":true,"description":"ADR-113 rule->ingest","parameters":{"method":"post","headers":{"x-tenant-id":"'"$TENANT_ID"'","x-ingest-key":"'"$INGEST_KEY"'"},"body":"{\"deviceId\":\"${deviceId}\",\"ts\":${ts},\"metrics\":{\"temp\":${temp}}}","max_retries":2}}' 
A_OUT=$(post_json "actions" "$ACTION_JSON")
A_CODE=$(tail -1 <<<"$A_OUT")
echo "      action    -> HTTP $A_CODE"
[ "$A_CODE" = "201" ] || [ "$A_CODE" = "200" ] && chk "action-created" 0 || chk "action-created" 1
[ "$A_CODE" = "201" ] || [ "$A_CODE" = "200" ] || echo "      $(head -c 300 <<<"$A_OUT")"

RULE_JSON='{"id":"'"$RULE_ID"'","enable":true,"description":"ADR-113 rule->ingest","sql":"SELECT payload.deviceId as deviceId, payload.ts as ts, payload.metrics.temp as temp FROM \"tenants/+/devices/+/telemetry\"","actions":["http:'"$ACTION_NAME"'"]}' 
# 守卫：确认 action 用的是本轮租户。若上面删除失败而沿用旧 action，这里会立刻暴露，
# 否则要等到「计量 0 / 设备归错租户」才被发现，排查成本高得多。
ACT_TID=$(curl -s -m 10 --noproxy '*' "${AUTH[@]}" "$EMQX_REST/api/v5/actions/http:$ACTION_NAME" \
  | grep -ao '"x-tenant-id":"[^"]*"' | head -1 | cut -d'"' -f4)
echo "      action.X-Tenant-Id = ${ACT_TID:-<空>}"
[ "$ACT_TID" = "$TENANT_ID" ] && chk "action-tenant-header-fresh" 0 || chk "action-tenant-header-fresh" 1

R_OUT=$(post_json "rules" "$RULE_JSON")
R_CODE=$(tail -1 <<<"$R_OUT")
echo "      rule      -> HTTP $R_CODE"
[ "$R_CODE" = "201" ] || [ "$R_CODE" = "200" ] && chk "rule-created" 0 || chk "rule-created" 1
[ "$R_CODE" = "201" ] || [ "$R_CODE" = "200" ] || echo "      $(head -c 300 <<<"$R_OUT")"

# ---------------- 5) 只经 MQTT 发布一条遥测（桥接关闭 => 只能靠规则引擎落库） ----------------
echo
echo "== 5) MQTT 发布（SaaS 侧订阅桥应为关闭状态）=="
DEVICE_ID=$(python -c "import uuid;print(uuid.uuid4())" 2>/dev/null || cat /proc/sys/kernel/random/uuid 2>/dev/null)
TOPIC="tenants/$TENANT_ID/devices/$DEVICE_ID/telemetry"
PAYLOAD="{\"deviceId\":\"$DEVICE_ID\",\"ts\":$(date +%s),\"metrics\":{\"temp\":77.7}}"
PUB=$(python scripts/mqtt_publish.py 127.0.0.1 "$EMQX_MQTT_PORT" "$TOPIC" "$PAYLOAD" 2>&1)
echo "      $PUB"
case "$PUB" in MQTT-PUBLISH-OK*) chk "mqtt-publish" 0;; *) chk "mqtt-publish" 1;; esac

# ---------------- 6) 断言：规则命中 + 落库 ----------------
echo
echo "== 6) 断言 =="
MET=""
for _ in $(seq 1 12); do
  sleep 3
  MET=$(curl -s -m 10 --noproxy '*' "${AUTH[@]}" "$EMQX_REST/api/v5/rules/$RULE_ID/metrics")
  M=$(grep -ao '"matched":[0-9]*' <<<"$MET" | head -1 | cut -d: -f2)
  S=$(grep -ao '"actions.success":[0-9]*' <<<"$MET" | head -1 | cut -d: -f2)
  [ "${M:-0}" -ge 1 ] 2>/dev/null && [ "${S:-0}" -ge 1 ] 2>/dev/null && break
done
P=$(grep -ao '"passed":[0-9]*' <<<"$MET" | head -1 | cut -d: -f2)
echo "      matched=${M:-0} passed=${P:-0} actions.success=${S:-0} actions.failed=$(grep -ao '"actions.failed":[0-9]*' <<<"$MET" | head -1 | cut -d: -f2)"
[ "${M:-0}" -ge 1 ] 2>/dev/null && chk "rule-matched" 0 || chk "rule-matched" 1
[ "${S:-0}" -ge 1 ] 2>/dev/null && chk "webhook-action-success" 0 || chk "webhook-action-success" 1

# 排他证据：桥接关闭时该设备只可能由规则引擎写入
DEV_CNT=$(run_psql "select count(*) from devices where \"Id\"='$DEVICE_ID' and \"TenantId\"='$TENANT_ID';")
[ -n "$DEV_CNT" ] && [ "$DEV_CNT" -ge 1 ] 2>/dev/null && chk "device-created-by-rule-engine" 0 || chk "device-created-by-rule-engine" 1
echo "      devices(本租户 + 仅经MQTT/规则引擎的设备): ${DEV_CNT:-<查询失败>}"
if ! [ "${DEV_CNT:-0}" -ge 1 ] 2>/dev/null; then
  DEV_ANY=$(run_psql "select count(*) from devices where \"Id\"='$DEVICE_ID';")
  if [ "${DEV_ANY:-0}" -ge 1 ] 2>/dev/null; then
    echo "      !! 设备存在但**不属于本租户** —— 多半是 action 沿用了旧的 X-Tenant-Id 头（删除未生效）"
  fi
fi

# 计量（MeteringAgent 每 30s 批量 drain，这里轮询）
UM=""
for _ in $(seq 1 12); do
  sleep 5
  UM=$(run_psql "select coalesce(sum(\"TelemetryPoints\"),0) from usage_meters where \"TenantId\"='$TENANT_ID';")
  [ "${UM:-0}" -ge 1 ] 2>/dev/null && break
done
[ "${UM:-0}" -ge 1 ] 2>/dev/null && chk "metering-incremented" 0 || chk "metering-incremented" 1
echo "      usage_meters.\"TelemetryPoints\": ${UM:-<查询失败>}"

echo
echo "== 结果: $FAIL 项失败 =="
echo "   通过 $PASS 项。租户=$TENANT_ID 设备=$DEVICE_ID"
[ "$FAIL" -eq 0 ] && exit 0 || exit 1
