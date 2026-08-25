#!/usr/bin/env bash
# 端到端验证：宿主 WebApi（plc-host）经 NamedPipe StreamJsonRpc 代理到边缘网关（plc-gateway）。
# 启动网关 + 宿主，curl /api/gateway/* 端点，断言返回；退出码 0 = 全部 PASS。
set -u

REPO="$(cd "$(dirname "$0")/.." && pwd)"
GW_DIR="$REPO/src/plc-gateway/bin/Debug/net11.0"
HOST_DIR="$REPO/src/plc-host/bin/Debug/net11.0"
PORT=5088

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

echo "== 启动网关 (cwd=$GW_DIR) =="
cd "$GW_DIR"
dotnet ./PlcGateway.dll &
GW_PID=$!
echo "gw pid=$GW_PID"

echo "== 启动宿主 (cwd=$HOST_DIR, port=$PORT) =="
cd "$HOST_DIR"
ASPNETCORE_URLS="http://localhost:$PORT" dotnet ./plc-host.dll &
HOST_PID=$!
echo "host pid=$HOST_PID"

echo "== 等待宿主就绪 (探测 /sys/info) =="
for i in $(seq 1 40); do
  if curl -s -o /dev/null "http://localhost:$PORT/sys/info"; then echo "宿主就绪"; break; fi
  sleep 0.5
done

# 网关 NamedPipe 监听需要一点时间，等 5s 确保可读
sleep 5

failures=0
chk() { if [ "$2" = "0" ]; then echo "[webapi] PASS $1"; else echo "[webapi] FAIL $1"; failures=$((failures+1)); fi; }

echo "== GET /api/gateway/health =="
curl -s "http://localhost:$PORT/api/gateway/health"; echo

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

echo "== 结果: $failures 项失败 =="
exit $failures
