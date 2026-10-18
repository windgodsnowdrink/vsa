#!/usr/bin/env bash
# Phase 4 QA 门禁：SaaS 控制台端到端冒烟
# 固化 Phase 3 冒烟链路为常驻门禁，覆盖此前 e2e 未触达的普通租户登录路径与 /ops/pricing：
#   - 静态托管 SPA（/、/css/*、/js/app.js、/css/icons.svg）MIME 正确
#   - SPA 深链回退（MapFallbackToFile）对任何非 API 路径返回 index.html
#   - /api/v1/* 不被 SPA 回退遮蔽（ops/health 返回 JSON 而非 html）
#   - 普通租户：register → login → /billing/usage | /billing/plan | /tenant/members 全 200
#   - 超管：login(root/admin) → /ops/tenants | /ops/health | /ops/pricing | /ops/roles 全 200
#   - ADR-108 红线：usage/plan/pricing 响应体不得出现 price/currency/amount/money
# 退出码 = 失败项数（0 = 全 PASS）。
#
# 依赖：wslc 容器运行时（PG 5432 / EMQX 18083 已映射 localhost）。脚本不启停容器，
#       仅校验 PG 可达；EMQX 桥接默认关闭，非启动依赖（缺失仅告警）。
set -u

REPO="$(cd "$(dirname "$0")/.." && pwd)"
SAAS_DIR="$REPO/src/plc-saas"
BIN_DIR="$SAAS_DIR/bin/Debug/net11.0"
CANDIDATE_PORTS="${CANDIDATE_PORTS:-5000 5199 5299 5399 5499 5599 5699 5799 5899 5999}"

# 绕过本机 HTTP 代理对 localhost 的拦截（否则返回 502 假象）
export no_proxy='*' NO_PROXY='*'

cleanup() {
  echo "== 清理 SaaS 进程 =="
  [ -n "${SAAS_PID:-}" ] && kill "$SAAS_PID" 2>/dev/null
  # 跨平台兜底：Windows 用 powershell 精准杀；Linux 无 powershell 时静默跳过
  powershell.exe -NoProfile -Command "Get-CimInstance Win32_Process -Filter \"Name='dotnet.exe'\" | Where-Object { \$_.CommandLine -like '*plc-saas.dll*' } | ForEach-Object { Stop-Process -Id \$_.ProcessId -Force }" 2>/dev/null || true
}
trap cleanup EXIT

# —— HTTP 助手：回显 "CODE\nBODY"（code 在末行，便于解析）——
post()      { curl -s -w $'\n%{http_code}' -X POST "$1" -H "Content-Type: application/json" -d "$2"; }
post_auth() { curl -s -w $'\n%{http_code}' -X POST "$1" -H "Content-Type: application/json" -H "Authorization: Bearer $3" -d "$2"; }
get_auth()  { curl -s -w $'\n%{http_code}' -H "Authorization: Bearer $2" "$1"; }
get()       { curl -s -w $'\n%{http_code}' "$1"; }
code_of() { printf '%s' "$1" | tail -1; }
body_of() { printf '%s' "$1" | sed '$d'; }

banned_in() { # 含价格/货币/金额禁词返回 0（违规）
  printf '%s' "$1" | grep -iqE '(^|[^a-z0-9_])price([^a-z0-9_]|$)' \
    || printf '%s' "$1" | grep -iqE '(^|[^a-z0-9_])currency([^a-z0-9_]|$)' \
    || printf '%s' "$1" | grep -iqE '(^|[^a-z0-9_])amount([^a-z0-9_]|$)' \
    || printf '%s' "$1" | grep -iqE '(^|[^a-z0-9_])money([^a-z0-9_]|$)'
}
field() { # 提取 "field":"value"
  printf '%s' "$1" | grep -o "\"$2\"[[:space:]]*:[[:space:]]*\"[^\"]*\"" | head -1 | sed "s/.*:[[:space:]]*\"//; s/\"$//"
}

echo "== 校验依赖：PostgreSQL 监听 localhost:5432 =="
if ! (exec 3<>/dev/tcp/127.0.0.1/5432) 2>/dev/null; then
  echo "!! PostgreSQL 不可达（localhost:5432）。请先启动 wslc 容器：wslc start plc-saas-postgres"
  exit 3
fi
exec 3>&- 2>/dev/null || true
echo "   PostgreSQL 可达。"

echo "== 校验 EMQX（桥接默认关闭，非启动依赖；缺失仅告警）=="
if (exec 3<>/dev/tcp/127.0.0.1/18083) 2>/dev/null; then
  exec 3>&- 2>/dev/null || true
  echo "   EMQX dashboard 18083 可达。"
else
  echo "   [warn] EMQX 18083 不可达：若启用 Mqtt:Bridge:Enabled，请先 wslc start plc-saas-emqx"
fi

echo "== 构建 plc-saas (dotnet build) =="
( cd "$REPO" && dotnet build src/plc-saas -c Debug -v quiet )
if [ ! -f "$BIN_DIR/plc-saas.dll" ]; then
  echo "!! 构建产物缺失：$BIN_DIR/plc-saas.dll"
  exit 4
fi

echo "== 启动 plc-saas（端口自愈，避开 Windows 保留段）=="
PORT=""
for TRY in $CANDIDATE_PORTS; do
  echo "== 尝试端口 $TRY =="
  cd "$BIN_DIR"
  ASPNETCORE_URLS="http://localhost:$TRY" ASPNETCORE_ENVIRONMENT=Development dotnet ./plc-saas.dll &
  SAAS_PID=$!
  OK=0
  for i in $(seq 1 60); do
    if ! kill -0 "$SAAS_PID" 2>/dev/null; then break; fi
    c=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:$TRY/")
    if [ "$c" = "200" ]; then OK=1; break; fi
    sleep 0.5
  done
  if [ "$OK" = "1" ]; then PORT="$TRY"; echo "== 选定端口 $PORT =="; break; fi
  echo "    端口 $TRY 不可用，终止并重试"
  kill "$SAAS_PID" 2>/dev/null; wait "$SAAS_PID" 2>/dev/null
done
if [ -z "$PORT" ]; then
  echo "!! 所有候选端口均无法绑定（可能被 Windows 保留段占用），e2e 终止"
  exit 2
fi
sleep 2

BASE="http://localhost:$PORT"
failures=0
chk() { if [ "$2" = "0" ]; then echo "[saas] PASS $1"; else echo "[saas] FAIL $1"; failures=$((failures+1)); fi; }

# ===== 1. SPA 静态托管 =====
echo "== GET / (SPA 壳) =="
ROOT=$(get "$BASE/"); RC=$(code_of "$ROOT"); BD=$(body_of "$ROOT")
[ "$RC" = "200" ] && chk "root-status" 0 || chk "root-status" 1
printf '%s' "$BD" | grep -q 'id="app"' && chk "root-has-app" 0 || chk "root-has-app" 1
printf '%s' "$BD" | grep -q 'src="/js/app.js"' && chk "root-wired-appjs" 0 || chk "root-wired-appjs" 1

echo "== GET /css/app.css (text/css) =="
H=$(curl -s -D - -o /dev/null "$BASE/css/app.css")
[ "$(curl -s -o /dev/null -w "%{http_code}" "$BASE/css/app.css")" = "200" ] && chk "appcss-status" 0 || chk "appcss-status" 1
printf '%s' "$H" | grep -qi "content-type:.*text/css" && chk "appcss-mime" 0 || chk "appcss-mime" 1

echo "== GET /css/tokens.css =="
[ "$(curl -s -o /dev/null -w "%{http_code}" "$BASE/css/tokens.css")" = "200" ] && chk "tokenscss-status" 0 || chk "tokenscss-status" 1

echo "== GET /js/app.js (application/javascript) =="
H=$(curl -s -D - -o /dev/null "$BASE/js/app.js")
[ "$(curl -s -o /dev/null -w "%{http_code}" "$BASE/js/app.js")" = "200" ] && chk "appjs-status" 0 || chk "appjs-status" 1
printf '%s' "$H" | grep -qiE "content-type:.*(application/javascript|text/javascript)" && chk "appjs-mime" 0 || chk "appjs-mime" 1

echo "== GET /css/icons.svg (image/svg+xml, 含 <symbol) =="
ICONS=$(get "$BASE/css/icons.svg"); RC=$(code_of "$ICONS"); BD=$(body_of "$ICONS")
[ "$RC" = "200" ] && chk "icons-status" 0 || chk "icons-status" 1
printf '%s' "$BD" | grep -q "<symbol" && chk "icons-symbols" 0 || chk "icons-symbols" 1

echo "== GET /__saas_deeplink_probe (SPA 回退返回 index.html) =="
DP=$(get "$BASE/__saas_deeplink_probe"); RC=$(code_of "$DP"); BD=$(body_of "$DP")
[ "$RC" = "200" ] && chk "spa-fallback-status" 0 || chk "spa-fallback-status" 1
printf '%s' "$BD" | grep -q "<script" && chk "spa-fallback-index" 0 || chk "spa-fallback-index" 1

# ===== 2. 超管登录 + API 不被回退遮蔽 =====
echo "== 超管登录 (root/admin) =="
SUP=$(post "$BASE/api/v1/auth/login" '{"tenant":"root","email":"root","password":"admin"}')
RC=$(code_of "$SUP"); BD=$(body_of "$SUP")
[ "$RC" = "200" ] && chk "superadmin-login" 0 || { chk "superadmin-login" 1; echo "   body: $BD"; }
SUP_AT=$(field "$BD" accessToken)
[ -n "$SUP_AT" ] && chk "superadmin-token" 0 || chk "superadmin-token" 1

echo "== GET /api/v1/ops/health 必须返回 JSON（非 index.html）=="
H=$(get_auth "$BASE/api/v1/ops/health" "$SUP_AT"); RC=$(code_of "$H"); BD=$(body_of "$H")
[ "$RC" = "200" ] && chk "ops-health-status" 0 || chk "ops-health-status" 1
printf '%s' "$BD" | grep -q '"Healthy"' && chk "ops-health-json" 0 || chk "ops-health-json" 1
printf '%s' "$BD" | grep -qiE "<!doctype|<html" && chk "api-not-shadowed" 1 || chk "api-not-shadowed" 0

# ===== 3. 普通租户：register → login → 受限端点 =====
TS="$(date +%s)-$RANDOM"
SLUG="qa-$TS"
EMAIL="qa-$TS@example.com"
echo "== POST /api/v1/tenants (register) slug=$SLUG =="
REG=$(post "$BASE/api/v1/tenants" "{\"companyName\":\"QA Tenant\",\"slug\":\"$SLUG\",\"adminEmail\":\"$EMAIL\",\"adminPassword\":\"QaPassw0rd!\"}")
RC=$(code_of "$REG"); BD=$(body_of "$REG")
[ "$RC" = "200" ] && chk "register" 0 || { chk "register" 1; echo "   body: $BD"; }

echo "== 普通租户登录 ($SLUG) =="
TEN=$(post "$BASE/api/v1/auth/login" "{\"tenant\":\"$SLUG\",\"email\":\"$EMAIL\",\"password\":\"QaPassw0rd!\"}")
RC=$(code_of "$TEN"); BD=$(body_of "$TEN")
[ "$RC" = "200" ] && chk "tenant-login" 0 || { chk "tenant-login" 1; echo "   body: $BD"; }
TEN_AT=$(field "$BD" accessToken)

echo "== GET /api/v1/billing/usage =="
USG=$(get_auth "$BASE/api/v1/billing/usage" "$TEN_AT"); RC=$(code_of "$USG"); BD=$(body_of "$USG")
[ "$RC" = "200" ] && chk "usage-status" 0 || chk "usage-status" 1
printf '%s' "$BD" | grep -qi '"plan"' && chk "usage-has-plan" 0 || chk "usage-has-plan" 1
banned_in "$BD" && chk "usage-adr108" 1 || chk "usage-adr108" 0

echo "== GET /api/v1/billing/plan =="
PLN=$(get_auth "$BASE/api/v1/billing/plan" "$TEN_AT"); RC=$(code_of "$PLN"); BD=$(body_of "$PLN")
[ "$RC" = "200" ] && chk "plan-status" 0 || chk "plan-status" 1
banned_in "$BD" && chk "plan-adr108" 1 || chk "plan-adr108" 0

echo "== GET /api/v1/tenant/members =="
MEM=$(get_auth "$BASE/api/v1/tenant/members" "$TEN_AT"); RC=$(code_of "$MEM"); BD=$(body_of "$MEM")
[ "$RC" = "200" ] && chk "members-status" 0 || chk "members-status" 1
printf '%s' "$BD" | grep -q "$EMAIL" && chk "members-contains-admin" 0 || chk "members-contains-admin" 1

# ===== 4. 超管运营端点 =====
echo "== GET /api/v1/ops/tenants =="
OPT=$(get_auth "$BASE/api/v1/ops/tenants" "$SUP_AT"); RC=$(code_of "$OPT"); BD=$(body_of "$OPT")
[ "$RC" = "200" ] && chk "ops-tenants-status" 0 || chk "ops-tenants-status" 1
printf '%s' "$BD" | grep -q "$SLUG" && chk "ops-tenants-contains-new" 0 || chk "ops-tenants-contains-new" 1

echo "== GET /api/v1/ops/pricing (ADR-108) =="
PRC=$(get_auth "$BASE/api/v1/ops/pricing" "$SUP_AT"); RC=$(code_of "$PRC"); BD=$(body_of "$PRC")
[ "$RC" = "200" ] && chk "ops-pricing-status" 0 || chk "ops-pricing-status" 1
printf '%s' "$BD" | grep -qi '"plans"' && chk "ops-pricing-has-plans" 0 || chk "ops-pricing-has-plans" 1
banned_in "$BD" && chk "ops-pricing-adr108" 1 || chk "ops-pricing-adr108" 0

echo "== GET /api/v1/ops/roles =="
ROL=$(get_auth "$BASE/api/v1/ops/roles" "$SUP_AT"); RC=$(code_of "$ROL"); BD=$(body_of "$ROL")
[ "$RC" = "200" ] && chk "ops-roles-status" 0 || chk "ops-roles-status" 1
printf '%s' "$BD" | grep -q '"Admin"' && chk "ops-roles-has-admin" 0 || chk "ops-roles-has-admin" 1

echo "== 结果: $failures 项失败 =="
exit $failures
