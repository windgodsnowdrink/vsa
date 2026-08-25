#!/usr/bin/env bash
# plc-gateway 端到端 RPC 冒烟验证：
#   1) 启动 PlcGateway.dll（cwd=输出目录，加载插件 + manifest）
#   2) 运行 GatewayClient 连接 NamedPipe 并断言 listProtocols/listDevices/read/write
#   3) 捕获客户端退出码，拆除网关进程
# 退出码：客户端的退出码（0=全部 PASS）。
#
# 注意：Git Bash(msys2) 把绝对 Unix 路径参数传给 Windows dotnet.exe 时会被破坏，
# 因此一律用相对路径 + cd 进入目标目录的方式调用 dotnet。
set -uo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT"

GW_REL="src/plc-gateway/bin/Debug/net11.0"
CLIENT_REL="src/plc-gateway/GatewayClient/bin/Debug/net11.0/GatewayClient.dll"
GW_LOG="$(mktemp -t gw-smoke.XXXX.log)"

fail() { echo "::error::$1"; cleanup; exit 1; }

cleanup() {
  if [[ -n "${GW_PID:-}" ]] && kill -0 "$GW_PID" 2>/dev/null; then
    kill "$GW_PID" 2>/dev/null || true
    wait "$GW_PID" 2>/dev/null || true
  fi
}

[[ -f "$GW_REL/PlcGateway.dll" ]] || fail "未找到网关程序: $GW_REL/PlcGateway.dll（请先 dotnet build plc-gateway）"
[[ -f "$CLIENT_REL" ]] || fail "未找到客户端: $CLIENT_REL（请先 dotnet build GatewayClient）"

# 清理可能残留的上一次网关进程（其 NamedPipe 实例会把新网关挡在门外，
# 且会锁定 bin 下的 dll 导致后续构建 MSB3027）。Git Bash 的 pkill 抓不到
# Windows 的 dotnet.exe，改用 PowerShell 按命令行精准结束。
powershell.exe -NoProfile -Command "Get-CimInstance Win32_Process -Filter \"Name='dotnet.exe'\" | Where-Object { \$_.CommandLine -like '*PlcGateway.dll*' } | ForEach-Object { Stop-Process -Id \$_.ProcessId -Force }" 2>/dev/null || true

echo "== 启动网关 (cwd=$GW_REL) =="
cd "$REPO_ROOT/$GW_REL"
dotnet ./PlcGateway.dll > "$GW_LOG" 2>&1 &
GW_PID=$!
cd "$REPO_ROOT"

# 给网关启动 + 插件加载留时间（客户端自身也有 20s 连接重试作为兜底）
sleep 4

echo "== 运行 RPC 冒烟客户端 =="
set +e
( cd "$REPO_ROOT/src/plc-gateway/GatewayClient/bin/Debug/net11.0" && dotnet ./GatewayClient.dll --pipe=plc-gateway --timeout=20 )
CLIENT_RC=$?
set -e

echo "== 客户端退出码: $CLIENT_RC =="

if [[ $CLIENT_RC -ne 0 ]]; then
  echo "::error::RPC 冒烟失败，网关日志尾部："
  tail -n 30 "$GW_LOG" || true
fi

cleanup
rm -f "$GW_LOG" || true
exit "$CLIENT_RC"
