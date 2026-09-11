#!/usr/bin/env bash
# 端到端验证：前端 cockpit / devices / 根 SPA（/console/）接入真实后端 /api/gateway/* 。
# 构建网关与宿主（确保静态页面被复制到宿主输出），启动两者，断言页面与 API 可达。
# 退出码 0 = 全部 PASS。
#
# 端口说明：Windows（Hyper-V/WSL2）会保留若干 TCP 端口段，Kestrel 绑定到保留段会报
# 10013 (access denied)。脚本自动从候选端口列表中挑选首个可绑定的端口；也可通过
# PORT 环境变量强制指定单一端口（不自动回退）。
set -u

REPO="$(cd "$(dirname "$0")/.." && pwd)"
GW_DIR="$REPO/src/plc-gateway/bin/Debug/net11.0"
HOST_DIR="$REPO/src/plc-host/bin/Debug/net11.0"
CANDIDATE_PORTS="${CANDIDATE_PORTS:-5089 5095 8080 3000 7070 8888 9000 4000 6000 7077 7777 8800 9090 7100 7300 7500 7600 7800 7900 8000 8020 8050 8200 8081 7200 7075}"

cleanup() {
  echo "== 清理进程 =="
  [ -n "${GW_PID:-}" ] && kill "$GW_PID" 2>/dev/null
  [ -n "${HOST_PID:-}" ] && kill "$HOST_PID" 2>/dev/null
  powershell.exe -NoProfile -Command "Get-CimInstance Win32_Process -Filter \"Name='dotnet.exe'\" | Where-Object { \$_.CommandLine -like '*PlcGateway.dll*' -or \$_.CommandLine -like '*plc-host.dll*' } | ForEach-Object { Stop-Process -Id \$_.ProcessId -Force }" 2>/dev/null || true
}
trap cleanup EXIT

echo "== 清理残留网关/宿主进程 =="
powershell.exe -NoProfile -Command "Get-CimInstance Win32_Process -Filter \"Name='dotnet.exe'\" | Where-Object { \$_.CommandLine -like '*PlcGateway.dll*' -or \$_.CommandLine -like '*plc-host.dll*' } | ForEach-Object { Stop-Process -Id \$_.ProcessId -Force }" 2>/dev/null || true
sleep 1

echo "== 构建网关 (dotnet build plc-gateway) =="
( cd "$REPO" && dotnet build src/plc-gateway -c Debug -v quiet )
echo "== 构建宿主 (dotnet build plc-host，触发 cockpit 页面复制) =="
( cd "$REPO" && dotnet build src/plc-host -c Debug -v quiet )

echo "== 启动网关 (cwd=$GW_DIR) =="
cd "$GW_DIR"
dotnet ./PlcGateway.dll &
GW_PID=$!
echo "gw pid=$GW_PID"

# ---- 端口自动选择：挑选首个可绑定的端口启动宿主 ----
PORT="${PORT:-}"
if [ -n "$PORT" ]; then
  CANDIDATE_PORTS="$PORT"
fi
for TRY in $CANDIDATE_PORTS; do
  echo "== 尝试宿主端口 $TRY =="
  cd "$HOST_DIR"
  ASPNETCORE_URLS="http://localhost:$TRY" dotnet ./plc-host.dll &
  HPID=$!
  OK=0
  for i in $(seq 1 16); do
    if ! kill -0 "$HPID" 2>/dev/null; then break; fi
    code=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:$TRY/pages/cockpit.html")
    if [ "$code" = "200" ]; then OK=1; break; fi
    sleep 0.5
  done
  if [ "$OK" = "1" ]; then PORT="$TRY"; HOST_PID="$HPID"; echo "== 选定宿主端口 $PORT =="; break; fi
  echo "    端口 $TRY 不可用，终止并重试"
  kill "$HPID" 2>/dev/null; wait "$HPID" 2>/dev/null
done

if [ -z "$PORT" ]; then
  echo "!! 所有候选端口均无法绑定（可能被 Windows 保留段占用），e2e 终止"
  exit 2
fi

# 网关 NamedPipe 监听需要一点时间，等 5s 确保可读
sleep 5

failures=0
chk() { if [ "$2" = "0" ]; then echo "[cockpit] PASS $1"; else echo "[cockpit] FAIL $1"; failures=$((failures+1)); fi; }

echo "== GET /pages/cockpit.html (200 + 同源接线 /api/gateway/devices) =="
COCKPIT=$(curl -s "http://localhost:$PORT/pages/cockpit.html")
echo "$COCKPIT" | grep -q "/api/gateway/devices" && chk "cockpit-wired" 0 || chk "cockpit-wired" 1

echo "== GET /pages/devices.html (200 + 同源接线 /api/gateway/read) =="
DEVICES=$(curl -s "http://localhost:$PORT/pages/devices.html")
echo "$DEVICES" | grep -q "/api/gateway/read" && chk "devices-wired" 0 || chk "devices-wired" 1

echo "== GET /api/gateway/protocols =="
PROT=$(curl -s "http://localhost:$PORT/api/gateway/protocols")
echo "$PROT"
echo "$PROT" | grep -q "builtin.sim" && chk "protocols" 0 || chk "protocols" 1

echo "== GET /api/gateway/devices =="
DEV=$(curl -s "http://localhost:$PORT/api/gateway/devices")
echo "$DEV"
echo "$DEV" | grep -q "dev-sim-1" && chk "devices" 0 || chk "devices" 1

echo "== POST /api/gateway/read (dev-sim-1, start=0, count=2, type=3) =="
READ=$(curl -s -X POST "http://localhost:$PORT/api/gateway/read" -H "Content-Type: application/json" -d '{"deviceId":"dev-sim-1","start":0,"count":2,"type":3}')
echo "$READ"
echo "$READ" | grep -Eq '"deviceId"\s*:\s*"dev-sim-1"' && chk "read" 0 || chk "read" 1

echo "== POST /api/gateway/write (dev-sim-1, start=0, type=3, values=[11,22]) =="
WRITE=$(curl -s -X POST "http://localhost:$PORT/api/gateway/write" -H "Content-Type: application/json" -d '{"deviceId":"dev-sim-1","start":0,"type":3,"values":[11,22]}')
echo "$WRITE"
echo "$WRITE" | grep -Eq '"written"\s*:\s*2' && chk "write" 0 || chk "write" 1

echo "== 根 SPA 同源托管（/console/，对齐 cockpit 同源方案）=="
ROOTHDR=$(curl -s -D - -o /dev/null "http://localhost:$PORT/")
echo "$ROOTHDR" | grep -qi "location: /console/" && chk "console-root-redirect" 0 || chk "console-root-redirect" 1

CONSOLE=$(curl -s "http://localhost:$PORT/console/")
echo "$CONSOLE" | grep -q "js/app.js" && chk "console-index" 0 || chk "console-index" 1

APPJS=$(curl -s "http://localhost:$PORT/console/js/app.js")
echo "$APPJS" | grep -q "/api/gateway/devices" && chk "console-appjs-wired" 0 || chk "console-appjs-wired" 1

DEVHTML=$(curl -s "http://localhost:$PORT/console/pages/devices.html")
echo "$DEVHTML" | grep -q 'data-page="devices"' && chk "console-devices-fragment" 0 || chk "console-devices-fragment" 1

echo "== Tier1 插件体系（ADR-118）：/plugins 与 /api/devices/* 目录 =="
PLUGINS=$(curl -s "http://localhost:$PORT/plugins")
echo "$PLUGINS"
echo "$PLUGINS" | grep -q "modbus.tcp" && chk "plugins-loaded" 0 || chk "plugins-loaded" 1
echo "$PLUGINS" | grep -q "melsec.mc" && chk "plugins-melsec" 0 || chk "plugins-melsec" 1

DP=$(curl -s "http://localhost:$PORT/api/devices/protocols")
echo "$DP"
echo "$DP" | grep -q "modbus.tcp" && chk "devices-protocols" 0 || chk "devices-protocols" 1
echo "$DP" | grep -q "melsec.mc" && chk "devices-protocols-melsec" 0 || chk "devices-protocols-melsec" 1

echo "== POST /api/devices/modbus.tcp/read（死主机 127.0.0.1:1，验证路由到已注册插件而非 404）=="
READCODE=$(curl -s -o /dev/null -w "%{http_code}" -X POST "http://localhost:$PORT/api/devices/modbus.tcp/read" -H "Content-Type: application/json" -d '{"host":"127.0.0.1","port":1,"unitId":1,"startAddress":0,"count":2,"type":3,"timeoutMs":1000}')
echo "read status=$READCODE（非 404 即证明插件已注册并被路由到，连接失败返回 500 属预期）"
if [ "$READCODE" != "404" ]; then chk "devices-read-routing" 0; else chk "devices-read-routing" 1; fi

echo "== 结果: $failures 项失败 =="
exit $failures
