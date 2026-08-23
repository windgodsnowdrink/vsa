# =============================================================================
# start-dev-env.ps1  —  本地开发环境启动（WSLC 容器，不用 Docker）
# -----------------------------------------------------------------------------
# 用 wslc（WSL 预览版容器 CLI）拉起 PostgreSQL + EMQX 两个本地容器，
# 供 plc-saas 后端运行期验证（health 端点 + 核心多租户流程 + V1-now MQTT 桥）。
# 镜像源：Docker Hub(registry-1.docker.io) 在本机/沙箱不可达，统一改用 docker.1ms.run 镜像加速器。
#
# 凭据（按总监裁定）：
#   PostgreSQL : localhost:5432  postgres / postgres   （库 plc_aiot_saas）
#   EMQX       : admin / Admin123  （MQTT 1883 · 控制台 18083 · WS 8083/8084）
#
# 端口发布（宿主侧 → 容器侧）：
#   默认宿主端口 5432（符合裁定）。若本机 5432 被占用 / 被 Windows 端口排除范围覆盖，
#   会以 WSAEACCES 失败。此时可临时覆盖宿主端口（容器内部仍是 5432，无需改镜像）：
#     $env:PG_HOST_PORT = '5433'      # 然后相应把 appsettings 连接串 Port 改为 5433
#   脚本启动前会自检端口是否可绑定，失败会打印排查指引而非 cryptic 报错。
#
# 用法：在仓库根目录执行
#   pwsh plc-saas/start-dev-env.ps1
# 停止：
#   wslc stop plc-saas-postgres plc-saas-emqx
# =============================================================================

$ErrorActionPreference = 'Stop'

$PG_NAME    = 'plc-saas-postgres'
$EMQX_NAME  = 'plc-saas-emqx'
$PG_IMAGE   = 'docker.1ms.run/library/postgres:18-alpine3.23'  # docker.1ms.run 镜像源（Docker Hub 不可达）
$EMQX_IMAGE = 'docker.1ms.run/emqx/emqx:6.1.4'                # docker.1ms.run 镜像源（EMQX 6.x）
$PG_VOLUME  = 'plc-saas-pgdata'
$PG_HOST_PORT = if ($env:PG_HOST_PORT) { $env:PG_HOST_PORT } else { '5432' }  # 宿主侧发布端口（容器内部恒为 5432）

function Container-Exists($name) {
    $out = wslc ps -a 2>$null
    return ($out -match [regex]::Escape($name))
}
function Container-Running($name) {
    $out = wslc ps 2>$null
    return ($out -match [regex]::Escape($name))
}

# 宿主端口可绑定性自检：尝试在 127.0.0.1 上 bind 一下，能绑说明可发布，不能绑则说明
# 已被占用或落在 Windows TCP 排除保留范围（这正是 wslc 报 WSAEACCES 的两类根因）。
function Assert-PortBindable($port) {
    try {
        $ep = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Loopback, [int]$port)
        $sock = [System.Net.Sockets.Socket]::new(
            [System.Net.Sockets.AddressFamily]::InterNetwork,
            [System.Net.Sockets.SocketType]::Stream,
            [System.Net.Sockets.ProtocolType]::Tcp)
        $sock.Bind($ep)
        $sock.Close()
        return $true
    } catch {
        return $false
    }
}

function Ensure-Postgres {
    if (Container-Running $PG_NAME) { Write-Host "[pg] 已在运行，跳过"; return }
    if (Container-Exists $PG_NAME) { Write-Host "[pg] 已存在，启动中…"; wslc start $PG_NAME; return }

    # 宿主端口可绑定性自检（避开 WSAEACCES cryptic 报错，给出可读排查指引）
    if (-not (Assert-PortBindable $PG_HOST_PORT)) {
        Write-Warning "宿主端口 ${PG_HOST_PORT} 无法绑定（WSAEACCES）。两类根因：(1) 已被占用——本机可能装有原生 PostgreSQL 服务在监听；(2) 端口落在 Windows TCP 排除保留范围（Hyper-V/WSL 自动保留）。"
        Write-Host "排查占用： Get-NetTCPConnection -LocalPort $PG_HOST_PORT | Select-Object OwningProcess,State ；随后 Stop-Service -Name postgresql-* -Force"
        Write-Host "排查保留： netsh interface ipv4 show excludedportrange protocol=tcp"
        Write-Host "修复保留（管理员 PowerShell）： netsh int ipv4 add excludedportrange protocol=tcp startport=$PG_HOST_PORT numberofports=1 store=persistent"
        Write-Host "或临时平移宿主端口（容器内部仍 5432）： `$env:PG_HOST_PORT='5433' ，并相应把 appsettings 连接串 Port 改为 5433"
        throw "宿主端口 ${PG_HOST_PORT} 不可绑定，已终止。请按上面指引处理后重试。"
    }

    Write-Host "[pg] 拉取镜像 $PG_IMAGE …"
    wslc pull $PG_IMAGE 2>&1 | Out-String -Width 200 | Write-Host

    # 持久化数据卷（best-effort，已存在则忽略报错）
    wslc volume create $PG_VOLUME 2>$null

    Write-Host "[pg] 启动容器（POSTGRES_DB 自动建库 plc_aiot_saas），宿主端口 ${PG_HOST_PORT} → 容器 5432…"
    wslc run -d --name $PG_NAME `
        --volume "${PG_VOLUME}:/var/lib/postgresql/data" `
        -e POSTGRES_USER=postgres `
        -e POSTGRES_PASSWORD=postgres `
        -e POSTGRES_DB=plc_aiot_saas `
        -p "${PG_HOST_PORT}:5432" `
        $PG_IMAGE
}

function Ensure-Emqx {
    if (Container-Running $EMQX_NAME) { Write-Host "[emqx] 已在运行，跳过"; return }
    if (Container-Exists $EMQX_NAME) { Write-Host "[emqx] 已存在，启动中…"; wslc start $EMQX_NAME; return }

    Write-Host "[emqx] 拉取镜像 $EMQX_IMAGE …"
    wslc pull $EMQX_IMAGE 2>&1 | Out-String -Width 200 | Write-Host

    Write-Host "[emqx] 启动容器（admin/Admin123，MQTT 1883 · 控制台 18083）…"
    wslc run -d --name $EMQX_NAME `
        -e EMQX_DASHBOARD__DEFAULT_USER__USERNAME=admin `
        -e EMQX_DASHBOARD__DEFAULT_USER__PASSWORD=Admin123 `
        -p 1883:1883 `
        -p 8083:8083 `
        -p 8084:8084 `
        -p 18083:18083 `
        $EMQX_IMAGE
}

Ensure-Postgres
Ensure-Emqx

Write-Host ""
Write-Host "=== 容器状态 ==="
wslc ps -a
Write-Host ""
Write-Host "下一步："
Write-Host "  1) 启动后端：dotnet run --project plc-saas/plc-saas.csproj   (EnsureCreated + rls.sql 自动建表/RLS)"
Write-Host "  2) 健康检查：curl -u <admin-token> http://localhost:5000/api/v1/ops/health"
Write-Host "  3) EMQX 控制台：http://localhost:18083  (admin / Admin123)"
