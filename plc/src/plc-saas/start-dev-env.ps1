# =============================================================================
# start-dev-env.ps1  —  本地开发环境启动（WSLC 容器，不用 Docker Desktop）
# -----------------------------------------------------------------------------
# 用 wslc（WSL 预览版容器 CLI）拉起 PostgreSQL + EMQX 两个本地容器，
# 供 plc-saas 后端运行期验证（health 端点 + 核心多租户流程 + ADR-113 双摄取路径）。
#
# ⚠️ 单一真源：docker-compose.yml
#    本脚本**不重复定义**镜像版本 / PG 凭据 / 端口，启动时会解析同目录下的
#    docker-compose.yml 取得这些值（解析失败则用下方 $Fallback 兜底）。
#    唯一差异是镜像名加 `docker.1ms.run` 镜像源前缀（Docker Hub 在本机不可达）。
#    → 改版本或凭据只改 docker-compose.yml，两条起服务路径自动一致。
#
# 凭据（与 docker-compose.yml 一致）：
#   PostgreSQL  superuser : postgresql / postgresql   （库 plc_aiot_saas）
#   PostgreSQL  应用角色  : plcsaas / plcsaas         （脚本等效创建；appsettings 连接串使用）
#   EMQX        console   : admin / Admin123          （MQTT 1883 · 控制台 18083 · WS 8083/8084 · SSL 8883）
#
# 端口发布（宿主侧 → 容器侧）：
#   默认宿主端口 5432。若本机 5432 被占用 / 被 Windows 端口排除范围覆盖，
#   会以 WSAEACCES 失败。此时可临时覆盖宿主端口（容器内部仍是 5432，无需改镜像）：
#     $env:PG_HOST_PORT = '55432'    # 然后相应把连接串 Port 改为 55432
#     # 即：ConnectionStrings__Default="Host=localhost;Port=55432;Database=plc_aiot_saas;Username=plcsaas;Password=plcsaas"
#   脚本启动前会自检端口是否可绑定，失败会打印排查指引而非 cryptic 报错。
#
# 用法：在仓库根目录执行
#   pwsh src/plc-saas/start-dev-env.ps1
# 停止：
#   wslc stop plc-saas-postgres plc-saas-emqx
# =============================================================================

$ErrorActionPreference = 'Stop'

$PG_NAME    = 'plc-saas-postgres'
$EMQX_NAME  = 'plc-saas-emqx'
$PG_VOLUME  = 'plc-saas-pgdata'
$PG_HOST_PORT = if ($env:PG_HOST_PORT) { $env:PG_HOST_PORT } else { '5432' }  # 宿主侧发布端口（容器内部恒为 5432）
$COMPOSE    = Join-Path $PSScriptRoot 'docker-compose.yml'

# ---------------- 从 docker-compose.yml 解析基础设施真值（带兜底） ----------------
# 解析失败（文件缺失 / 格式变更）时退化为下列硬编码值，保证脚本始终可用。
$Infra = @{
    PgVersion      = '16'
    EmqxVersion    = '5.8.5'
    PgSuperUser    = 'postgresql'
    PgSuperPwd     = 'postgresql'
    PgDb           = 'plc_aiot_saas'
    PgAppUser      = 'plcsaas'
    PgAppPwd       = 'plcsaas'
    EmqxUser       = 'admin'
    EmqxPwd        = 'Admin123'
}

function Read-Infra {
    if (-not (Test-Path $COMPOSE)) {
        Write-Warning "未找到 $COMPOSE，使用内置兜底值（Pg$($Infra.PgVersion) / EMQX $($Infra.EmqxVersion)）。"
        return
    }
    foreach ($line in (Get-Content -LiteralPath $COMPOSE)) {
        if ($line -match '^\s*image:\s*postgres:(.+?)\s*$')        { $Infra.PgVersion   = $Matches[1] }
        elseif ($line -match '^\s*image:\s*emqx/emqx:(.+?)\s*$')   { $Infra.EmqxVersion = $Matches[1] }
        elseif ($line -match '^\s*POSTGRES_USER:\s*(.+?)\s*$')     { $Infra.PgSuperUser = $Matches[1] }
        elseif ($line -match '^\s*POSTGRES_PASSWORD:\s*(.+?)\s*$') { $Infra.PgSuperPwd  = $Matches[1] }
        elseif ($line -match '^\s*POSTGRES_DB:\s*(.+?)\s*$')       { $Infra.PgDb        = $Matches[1] }
        elseif ($line -match '^\s*EMQX_DASHBOARD__DEFAULT_USERNAME:\s*(.+?)\s*$') { $Infra.EmqxUser = $Matches[1] }
        elseif ($line -match '^\s*EMQX_DASHBOARD__DEFAULT_PASSWORD:\s*(.+?)\s*$') { $Infra.EmqxPwd  = $Matches[1] }
    }
    Write-Host "[cfg] 取自 docker-compose.yml —— PG $($Infra.PgVersion)（superuser $($Infra.PgSuperUser)） · EMQX $($Infra.EmqxVersion)（$($Infra.EmqxUser)）"
}
Read-Infra

# docker.1ms.run 镜像源前缀（Docker Hub 在本机/沙箱不可达）
$PG_IMAGE   = "docker.1ms.run/library/postgres:$($Infra.PgVersion)"
$EMQX_IMAGE = "docker.1ms.run/emqx/emqx:$($Infra.EmqxVersion)"

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

function Wait-PostgresReady {
    Write-Host "[pg] 等待就绪（pg_isready）…"
    for ($i = 0; $i -lt 30; $i++) {
        try {
            $out = wslc exec $PG_NAME pg_isready -U $Infra.PgSuperUser -d $Infra.PgDb 2>&1 | Out-String
            if ($out -match 'accepting connections') { Write-Host "[pg] 就绪"; return $true }
        } catch { }
        Start-Sleep -Seconds 2
    }
    Write-Warning "[pg] 30 次探测仍未就绪，请检查容器日志： wslc logs $PG_NAME"
    return $false
}

# 应用角色 plcsaas：compose 路径由 initdb/01-roles.sql 在初始化阶段创建；
# wslc 路径无 initdb 挂载，这里用 psql 等效创建，保证两条路径凭据完全一致。
function Ensure-AppRole {
    $exists = ''
    try {
        $exists = (wslc exec $PG_NAME psql -U $Infra.PgSuperUser -d $Infra.PgDb -tAc "SELECT 1 FROM pg_roles WHERE rolname='$($Infra.PgAppUser)'" 2>$null | Out-String).Trim()
    } catch { }
    if ($exists -eq '1') { Write-Host "[pg] 应用角色 $($Infra.PgAppUser) 已存在，跳过"; return }

    Write-Host "[pg] 创建应用角色 $($Infra.PgAppUser)（等效于 initdb/01-roles.sql）…"
    $sql = "DO `$`$ BEGIN IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = '$($Infra.PgAppUser)') THEN CREATE ROLE $($Infra.PgAppUser) LOGIN PASSWORD '$($Infra.PgAppPwd)' SUPERUSER; END IF; END `$`$;"
    try {
        wslc exec $PG_NAME psql -U $Infra.PgSuperUser -d $Infra.PgDb -c $sql 2>&1 | Out-String -Width 200 | Write-Host
    } catch {
        Write-Warning "自动创建应用角色失败，请手动执行："
        Write-Host "  wslc exec $PG_NAME psql -U $($Infra.PgSuperUser) -d $($Infra.PgDb) -c `"$sql`""
    }
}

function Ensure-Postgres {
    if (Container-Running $PG_NAME) { Write-Host "[pg] 已在运行，跳过"; Ensure-AppRole; return }
    if (Container-Exists $PG_NAME) { Write-Host "[pg] 已存在，启动中…"; wslc start $PG_NAME; Wait-PostgresReady | Out-Null; Ensure-AppRole; return }

    # 宿主端口可绑定性自检（避开 WSAEACCES cryptic 报错，给出可读排查指引）
    if (-not (Assert-PortBindable $PG_HOST_PORT)) {
        Write-Warning "宿主端口 ${PG_HOST_PORT} 无法绑定（WSAEACCES）。两类根因：(1) 已被占用——本机可能装有原生 PostgreSQL 服务在监听；(2) 端口落在 Windows TCP 排除保留范围（Hyper-V/WSL 自动保留）。"
        Write-Host "排查占用： Get-NetTCPConnection -LocalPort $PG_HOST_PORT | Select-Object OwningProcess,State ；随后 Stop-Service -Name postgresql-* -Force"
        Write-Host "排查保留： netsh interface ipv4 show excludedportrange protocol=tcp"
        Write-Host "修复保留（管理员 PowerShell）： netsh int ipv4 add excludedportrange protocol=tcp startport=$PG_HOST_PORT numberofports=1 store=persistent"
        Write-Host "或临时平移宿主端口（容器内部仍 5432）： `$env:PG_HOST_PORT='55432' ，并相应把连接串 Port 改为 55432"
        throw "宿主端口 ${PG_HOST_PORT} 不可绑定，已终止。请按上面指引处理后重试。"
    }

    Write-Host "[pg] 拉取镜像 $PG_IMAGE …"
    wslc pull $PG_IMAGE 2>&1 | Out-String -Width 200 | Write-Host

    # 持久化数据卷（best-effort，已存在则忽略报错）
    wslc volume create $PG_VOLUME 2>$null

    Write-Host "[pg] 启动容器（POSTGRES_DB 自动建库 $($Infra.PgDb)），宿主端口 ${PG_HOST_PORT} → 容器 5432…"
    wslc run -d --name $PG_NAME `
        --volume "${PG_VOLUME}:/var/lib/postgresql/data" `
        -e "POSTGRES_USER=$($Infra.PgSuperUser)" `
        -e "POSTGRES_PASSWORD=$($Infra.PgSuperPwd)" `
        -e "POSTGRES_DB=$($Infra.PgDb)" `
        -p "${PG_HOST_PORT}:5432" `
        $PG_IMAGE

    Wait-PostgresReady | Out-Null
    Ensure-AppRole
}

function Wait-EmqxReady {
    Write-Host "[emqx] 等待控制台就绪（http://localhost:18083）…"
    for ($i = 0; $i -lt 30; $i++) {
        try {
            $r = Invoke-WebRequest -Uri 'http://localhost:18083' -UseBasicParsing -TimeoutSec 3 -SkipHttpErrorCheck
            if ($r.StatusCode -ge 200 -and $r.StatusCode -lt 500) { Write-Host "[emqx] 就绪"; return $true }
        } catch { }
        Start-Sleep -Seconds 2
    }
    Write-Warning "[emqx] 控制台未就绪，请检查容器日志： wslc logs $EMQX_NAME"
    return $false
}

function Ensure-Emqx {
    if (Container-Running $EMQX_NAME) { Write-Host "[emqx] 已在运行，跳过"; return }
    if (Container-Exists $EMQX_NAME) { Write-Host "[emqx] 已存在，启动中…"; wslc start $EMQX_NAME; Wait-EmqxReady | Out-Null; return }

    Write-Host "[emqx] 拉取镜像 $EMQX_IMAGE …"
    wslc pull $EMQX_IMAGE 2>&1 | Out-String -Width 200 | Write-Host

    Write-Host "[emqx] 启动容器（$($Infra.EmqxUser)/$($Infra.EmqxPwd)，MQTT 1883 · 控制台 18083）…"
    wslc run -d --name $EMQX_NAME `
        -e "EMQX_DASHBOARD__DEFAULT_USERNAME=$($Infra.EmqxUser)" `
        -e "EMQX_DASHBOARD__DEFAULT_PASSWORD=$($Infra.EmqxPwd)" `
        -e "EMQX_NODE__NAME=emqx@127.0.0.1" `
        -p 1883:1883 `
        -p 8083:8083 `
        -p 8084:8084 `
        -p 8883:8883 `
        -p 18083:18083 `
        $EMQX_IMAGE

    Wait-EmqxReady | Out-Null
}

Ensure-Postgres
Ensure-Emqx

Write-Host ""
Write-Host "=== 容器状态 ==="
wslc ps -a
Write-Host ""
Write-Host "下一步："
Write-Host "  1) 启动后端：dotnet run --project src/plc-saas/plc-saas.csproj   (EnsureCreated + rls.sql 自动建表/RLS)"
Write-Host "     连接串默认已对齐容器：Host=localhost;Port=$PG_HOST_PORT;Database=$($Infra.PgDb);Username=$($Infra.PgAppUser);Password=$($Infra.PgAppPwd)"
Write-Host "     （宿主端口若非 5432，用 ConnectionStrings__Default 环境变量覆盖 Port）"
Write-Host "  2) 健康检查：curl http://localhost:5000/api/v1/ops/health"
Write-Host "  3) EMQX 控制台：http://localhost:18083  ($($Infra.EmqxUser) / $($Infra.EmqxPwd))"
Write-Host "  4) 实测：bash scripts/verify-emqx-e2e.sh"
