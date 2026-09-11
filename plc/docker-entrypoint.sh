#!/usr/bin/env bash
# plc-host 容器入口：先起边缘网关（NamedPipe: plc-gateway），待其就绪后起 WebApi 宿主。
# 同一容器内运行，Linux 下 .NET NamedPipe 映射为 Unix 套接字，宿主可直连网关。
set -euo pipefail

GATEWAY_DLL=/app/gateway/PlcGateway.dll
HOST_DLL=/app/host/plc-host.dll

# 后台启动边缘网关
dotnet "$GATEWAY_DLL" &
GATEWAY_PID=$!

cleanup() {
  echo "== 关闭网关与宿主 =="
  kill "$GATEWAY_PID" 2>/dev/null || true
}
trap cleanup EXIT INT TERM

# 等待 NamedPipe / Unix 套接字就绪（演示用固定等待；生产可改为探测套接字路径）
sleep 3

# 前台启动 WebApi 宿主（含 /console/ SPA 与 /api/gateway/*、/api/devices/*）
dotnet "$HOST_DLL"
