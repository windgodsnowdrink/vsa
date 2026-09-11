#!/usr/bin/env bash
# 探测 OS 层可绑定的本地端口（Windows 保留段会导致 10013），输出第一个可用端口。
# 使用 powershell.exe 的 TcpListener 做真实 bind 试探。
set -u
powershell.exe -NoProfile -Command '& { $ps=@(8080,7070,3000,8888,9000,4000,6000,7077,7777,8800,9090,7100,7300,7500,7600,7800,7900,8000,8020,8050,8200,8081,7200,7075); foreach($p in $ps){ $l=$null; try { $l=New-Object System.Net.Sockets.TcpListener([System.Net.IPAddress]::Loopback,$p); $l.Start(); $l.Stop(); Write-Host "OK $p"; break } catch { Write-Host "BAD $p" } } }'
echo "---done---"
