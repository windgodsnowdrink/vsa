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
#   EMQX        console   : admin / Admin123          （控制台 18083）
#
# 端口发布（宿主侧 → 容器侧）：
#   EMQX 部分宿主端口平移到 5xxxx —— 默认宿主端口 1883 落在 Windows 排除保留段
#   1802-1901，8083/8084 落在 8035-8134，wslc 发布时报 WSAEACCES(10013)。映射为：
#       MQTT 1883 → 51883    WS 8083 → 58083    WSS 8084 → 58084
#       TLS 8883 保持 8883、控制台 18083 保持 18083（均不在保留段，实测可绑定）
#   容器内端口始终不变，镜像与 EMQX 配置无需改。可用环境变量覆盖：
#       EMQX_MQTT_HOST_PORT / EMQX_WS_HOST_PORT / EMQX_WSS_HOST_PORT /
#       EMQX_TLS_HOST_PORT / EMQX_DASHBOARD_HOST_PORT
#   环境变量显式指定时不参与“不可绑定自愈”，脚本无条件遵从。
#   默认宿主端口 5432。若本机 5432 被占用 / 被 Windows 端口排除范围覆盖，
#   会以 WSAEACCES 失败。此时可临时覆盖宿主端口（容器内部仍是 5432，无需改镜像）：
#     $env:PG_HOST_PORT = '55432'    # 然后相应把连接串 Port 改为 55432
#     # 即：ConnectionStrings__Default="Host=localhost;Port=55432;Database=plc_aiot_saas;Username=plcsaas;Password=plcsaas"
#   脚本启动前会自检端口是否可绑定，失败会打印排查指引而非 cryptic 报错。
#
# 用法：在仓库根目录执行
#   pwsh src/plc-saas/start-dev-env.ps1              # 起基础设施 + 自动跑 e2e 门禁
#   pwsh src/plc-saas/start-dev-env.ps1 -SkipE2E     # 只起基础设施，跳过 e2e 自检
# 停止：
#   wslc stop plc-saas-postgres plc-saas-emqx
# =============================================================================

param(
    # 启动完成（PG + EMQX 就绪）后自动跑 SaaS 控制台 e2e 门禁；
    # 只想单纯起基础设施时加 -SkipE2E 跳过自检。
    [switch]$SkipE2E
)

$ErrorActionPreference = 'Stop'
# 输出编码：本脚本全程中文提示。Windows 控制台默认走系统 ANSI 代码页，会让本脚本与
# Git Bash/wslc 回传的 UTF-8 文本变成乱码（"锟斤拷"）。这里统一切成 UTF-8。
# 必须设在脚本最前面：Invoke-E2EGate 等函数内部再设已来不及，前面的输出已经乱码了。
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }

$REPO_ROOT  = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$PG_NAME    = 'plc-saas-postgres'
$EMQX_NAME  = 'plc-saas-emqx'
$PG_VOLUME  = if ($env:PG_VOLUME) { $env:PG_VOLUME } else { 'plc-saas-pgdata' }
$PG_HOST_PORT = if ($env:PG_HOST_PORT) { $env:PG_HOST_PORT } else { '5432' }  # 宿主侧发布端口（容器内部恒为 5432）

# ---- EMQX 宿主侧端口平移（容器内端口不变，镜像/EMQX 配置无需改）----
# 根因：本机实测 Windows TCP 排除保留段会吞掉部分默认宿主端口，wslc 发布时
#       报 WSAEACCES（10013）：1883（落在 1802-1901）、8083/8084（落在 8035-8134）。
#       完整保留段实测：
#       80 · 4394-4493 · 5357 · 8035-8134 · 8235-8334 · 8443 · 9103-9202 · 13508 · 50000-50059。
# 对策（宿主 → 容器，与本机现存容器映射完全一致）：
#   MQTT 1883 → 51883    WS 8083 → 58083    WSS 8084 → 58084
#   TLS 8883 保持 8883、控制台 18083 保持 18083 —— 二者均不在保留段，实测可绑定，无需平移。
# 取宿主端口：环境变量优先（显式指定则无条件遵从，不自愈），否则取 compose 声明值，
# 若 compose 值不可绑定（落在 Windows 保留段 / 被占用）则回退到上面兜底值。
$HostFix = @{
    Mqtt      = '51883'
    Ws        = '58083'
    Wss       = '58084'
    Tls       = '8883'
    Dashboard = '18083'
}
$EnvPinned = @{
    Mqtt      = [bool]$env:EMQX_MQTT_HOST_PORT
    Ws        = [bool]$env:EMQX_WS_HOST_PORT
    Wss       = [bool]$env:EMQX_WSS_HOST_PORT
    Tls       = [bool]$env:EMQX_TLS_HOST_PORT
    Dashboard = [bool]$env:EMQX_DASHBOARD_HOST_PORT
}
if ($EnvPinned['Mqtt'])      { $HostFix['Mqtt']      = $env:EMQX_MQTT_HOST_PORT }
if ($EnvPinned['Ws'])        { $HostFix['Ws']        = $env:EMQX_WS_HOST_PORT }
if ($EnvPinned['Wss'])       { $HostFix['Wss']       = $env:EMQX_WSS_HOST_PORT }
if ($EnvPinned['Tls'])       { $HostFix['Tls']       = $env:EMQX_TLS_HOST_PORT }
if ($EnvPinned['Dashboard']) { $HostFix['Dashboard'] = $env:EMQX_DASHBOARD_HOST_PORT }

$COMPOSE    = Join-Path $PSScriptRoot 'docker-compose.yml'

# ---------------- 从 docker-compose.yml 解析基础设施真值（带兜底） ----------------
# 解析失败（文件缺失 / 格式变更）时退化为下列硬编码值，保证脚本始终可用。
$Infra = @{
    PgVersion      = '18-alpine3.23'   # 与 docker-compose.yml 保持一致（旧值 '16' 已陈旧）
    EmqxVersion    = '5.8.5'
    PgSuperUser    = 'postgresql'
    PgSuperPwd     = 'postgresql'
    PgDb           = 'plc_aiot_saas'
    PgAppUser      = 'plcsaas'
    PgAppPwd       = 'plcsaas'
    EmqxUser       = 'admin'
    EmqxPwd        = 'Admin123'
}

# compose 中声明的 EMQX 宿主端口（短语法 `- "宿主:容器"`）。解析不到即留空。
$ComposePorts = @{}

# 容器端口 → $HostFix 键
$ContainerPortMap = @{ '1883' = 'Mqtt'; '8083' = 'Ws'; '8084' = 'Wss'; '8883' = 'Tls'; '18083' = 'Dashboard' }
$curTarget = $null

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
        # compose 短语法 `- "宿主:容器"`；必须容忍行尾注释（`- "1883:1883"   # MQTT`），
        # 否则整行匹配失败，看起来就像"compose 没声明端口"（这正是最初踩到的坑）。
        elseif ($line -match '^\s*-\s*"?(\d+):1883"?\s*(#.*)?$')  { $ComposePorts['Mqtt']      = $Matches[1] }
        elseif ($line -match '^\s*-\s*"?(\d+):8083"?\s*(#.*)?$')  { $ComposePorts['Ws']        = $Matches[1] }
        elseif ($line -match '^\s*-\s*"?(\d+):8084"?\s*(#.*)?$')  { $ComposePorts['Wss']       = $Matches[1] }
        elseif ($line -match '^\s*-\s*"?(\d+):8883"?\s*(#.*)?$')  { $ComposePorts['Tls']       = $Matches[1] }
        elseif ($line -match '^\s*-\s*"?(\d+):18083"?\s*(#.*)?$') { $ComposePorts['Dashboard'] = $Matches[1] }
        # compose 长语法（`- target: <容器端口>` 后跟 `published: "<宿主端口>"`）
        elseif ($line -match '^\s*-\s*target:\s*(\d+)\s*$') { $curTarget = $Matches[1] }
        elseif ($line -match '^\s*published:\s*"?(\d+)"?\s*$' -and $curTarget) {
            $key = $ContainerPortMap[$curTarget]
            if ($key) { $ComposePorts[$key] = $Matches[1] }
            $curTarget = $null
        }
    }
    Write-Host "[cfg] 取自 docker-compose.yml —— PG $($Infra.PgVersion)（superuser $($Infra.PgSuperUser)） · EMQX $($Infra.EmqxVersion)（$($Infra.EmqxUser)）"
}
Read-Infra

# EMQX 宿主端口取值优先级：环境变量 > compose 声明值（须可绑定）> 5xxxx 兜底。
# compose 当前若仍声明 1883/8083/8084/8883，会因 Windows 保留段不可绑定而自动回退到
# 5xxxx 并告警；把 compose 同步为 5xxxx 后本步即为幂等透传。
function Resolve-EmqxHostPorts {
    foreach ($k in @('Mqtt', 'Ws', 'Wss', 'Tls', 'Dashboard')) {
        if ($EnvPinned[$k]) { continue }          # 环境变量显式指定 → 无条件遵从，不自愈
        $candidate = $ComposePorts[$k]
        if (-not $candidate -or $candidate -eq $HostFix[$k]) { continue }
        if (Assert-PortBindable $candidate) {
            Write-Host "[cfg] EMQX ${k} 宿主端口取自 compose：$candidate"
            $HostFix[$k] = $candidate
        } else {
            Write-Warning "[cfg] compose 声明的 EMQX ${k} 宿主端口 $candidate 不可绑定（Windows 保留段/被占用），回退到 $($HostFix[$k])。建议把 docker-compose.yml 同步为 5xxxx。"
        }
    }
    if ($ComposePorts.Count -eq 0) {
        Write-Host "[cfg] compose 未解析到 EMQX 宿主端口映射（需短语法 - `"宿主:容器`"），采用内置 5xxxx 映射；如需以 compose 为唯一真源，请按其短语法同步端口。"
    } else {
        Write-Host "[cfg] EMQX 宿主端口已与 compose 对齐：$((@('Mqtt','Ws','Wss','Tls','Dashboard') | ForEach-Object { "$_=$($HostFix[$_])" }) -join ' · ')"
    }
}
Resolve-EmqxHostPorts

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
    Write-Host "[emqx] 等待控制台就绪（http://localhost:$($HostFix['Dashboard'])）…"
    for ($i = 0; $i -lt 30; $i++) {
        try {
            $r = Invoke-WebRequest -Uri "http://localhost:$($HostFix['Dashboard'])" -UseBasicParsing -TimeoutSec 3 -SkipHttpErrorCheck
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

    # 宿主端口可绑定性自检（5 个端口一起验；任一个不可绑就带着排查指引终止，
    # 避免 wslc 报出无法归因的 WSAEACCES）。
    $bad = @()
    foreach ($k in @('Mqtt', 'Ws', 'Wss', 'Tls', 'Dashboard')) {
        if (-not (Assert-PortBindable $HostFix[$k])) { $bad += "${k}=$($HostFix[$k])" }
    }
    if ($bad.Count -gt 0) {
        Write-Warning "EMQX 宿主端口不可绑定：$($bad -join ', ')。两类根因：(1) 已被占用；(2) 落在 Windows TCP 排除保留范围。"
        Write-Host "排查占用： `$bad | ForEach-Object { Get-NetTCPConnection -LocalPort (`$_ -split '=')[1] -ErrorAction SilentlyContinue | Select-Object LocalPort,OwningProcess,State }"
        Write-Host "排查保留： netsh interface ipv4 show excludedportrange protocol=tcp"
        Write-Host "临时平移： `$env:EMQX_MQTT_HOST_PORT='51883'  /  `$env:EMQX_WS_HOST_PORT='58083'  /  `$env:EMQX_WSS_HOST_PORT='58084'  /  `$env:EMQX_DASHBOARD_HOST_PORT='18083'"
        throw "EMQX 宿主端口不可绑定，已终止： $($bad -join ', ')"
    }

    Write-Host "[emqx] 拉取镜像 $EMQX_IMAGE …"
    wslc pull $EMQX_IMAGE 2>&1 | Out-String -Width 200 | Write-Host

    Write-Host "[emqx] 启动容器（$($Infra.EmqxUser)/$($Infra.EmqxPwd)，宿主→容器： MQTT $($HostFix['Mqtt'])→1883 · WS $($HostFix['Ws'])→8083 · WSS $($HostFix['Wss'])→8084 · TLS $($HostFix['Tls'])→8883 · 控制台 $($HostFix['Dashboard'])→18083）…"
    wslc run -d --name $EMQX_NAME `
        -e "EMQX_DASHBOARD__DEFAULT_USERNAME=$($Infra.EmqxUser)" `
        -e "EMQX_DASHBOARD__DEFAULT_PASSWORD=$($Infra.EmqxPwd)" `
        -e "EMQX_NODE__NAME=emqx@127.0.0.1" `
        -p "$($HostFix['Mqtt']):1883" `
        -p "$($HostFix['Ws']):8083" `
        -p "$($HostFix['Wss']):8084" `
        -p "$($HostFix['Tls']):8883" `
        -p "$($HostFix['Dashboard']):18083" `
        $EMQX_IMAGE

    Wait-EmqxReady | Out-Null
}

# ---------------- Git Bash 解析（关键：不能用 Windows 的 System32\bash.exe）----------------
# ⚠️ PowerShell 里裸写 `bash` 极易命中 `C:\Windows\System32\bash.exe` —— 那是 **WSL 启动器**，
#    不是 Git Bash。它会去 spawn `wsl.exe`：WSL 未装发行版 / 被安全策略拦截时，只吐一段
#    不可读的信息并以非 0 退出，门禁脚本实则一行都没跑（表现为「1 项失败」的假象）。
#    本机 Git Bash 是 WorkBuddy 内置 PortableGit，不在标准 Program Files 路径下。
# 优先级：$env:E2E_BASH > WorkBuddy PortableGit > Program Files Git > PATH 中非 System32 者。
function Resolve-BashExe {
    if ($env:E2E_BASH -and (Test-Path -LiteralPath $env:E2E_BASH)) { return $env:E2E_BASH }

    $patterns = @(
        (Join-Path ${env:USERPROFILE} '.workbuddy/binaries/PortableGit/versions/*/bin/bash.exe')
        (Join-Path ${env:USERPROFILE} '.workbuddy/binaries/PortableGit/versions/*/usr/bin/bash.exe')
        (Join-Path ${env:ProgramFiles} 'Git/bin/bash.exe')
        (Join-Path ${env:ProgramFiles} 'Git/usr/bin/bash.exe')
    )
    if (${env:ProgramFiles(x86)}) {
        $patterns += (Join-Path ${env:ProgramFiles(x86)} 'Git/bin/bash.exe')
    }
    foreach ($p in $patterns) {
        $hit = Get-ChildItem -Path $p -ErrorAction SilentlyContinue |
               Sort-Object { [version]([regex]::Match($_.FullName, 'versions\\([^\\]+)\\').Groups[1].Value) } -Descending -ErrorAction SilentlyContinue |
               Select-Object -First 1
        if ($hit) { return $hit.FullName }
    }

    $cmd = Get-Command bash.exe -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($cmd -and $cmd.Source -notmatch '\\(System32|SysWOW64)\\') { return $cmd.Source }
    return $null
}

# ---------------- e2e 一键自检（Phase 4 QA 门禁） ----------------
# 基础设施就绪后即跑 SaaS 控制台端到端冒烟（33 项断言：SPA 静态托管 12 / 超管登录与
# /api 不被回退遮蔽 5 / 普通租户 register→login→受限端点 9 / 超管运营端点 7）。
# 退出码 = 失败项数，0 即全 PASS。仅需起基础设施时用 -SkipE2E 跳过。
function Invoke-E2EGate {
    $gate = Join-Path $REPO_ROOT 'scripts/verify-saas-console.sh'
    if (-not (Test-Path $gate)) {
        Write-Warning "[e2e] 未找到 $gate，跳过一键自检。"
        return
    }
    $bashExe = Resolve-BashExe
    if (-not $bashExe) {
        Write-Warning "[e2e] 未找到 Git Bash，跳过一键自检。请安装 Git for Windows，或用 `$env:E2E_BASH 指定 bash.exe 绝对路径。"
        return
    }
    $logDir = Join-Path $REPO_ROOT '.workbuddy\test'
    $log    = Join-Path $logDir 'e2e-gate.log'
    Write-Host ""
    Write-Host "=== e2e 一键自检：scripts/verify-saas-console.sh ==="
    # E2E_LOG 须为 POSIX 路径（Git Bash）；日志由 bash 侧 tee 落盘。
    # 不能用 PowerShell 捕获原生输出：控制台代码页转码会产生乱码并截断内容。
    $posixLog        = '/' + $log.Substring(0, 1).ToLower() + (($log.Substring(2)) -replace '\\', '/')
    $env:PG_PORT             = $PG_HOST_PORT
    $env:EMQX_DASHBOARD_PORT = $HostFix['Dashboard']
    $env:E2E_LOG             = $posixLog
    Push-Location $REPO_ROOT
    try {
        Write-Host "[e2e] bash：$bashExe"
        & $bashExe $gate   # 输出直连控制台，同时由脚本内 tee 写到 $E2E_LOG
        $code = $LASTEXITCODE
    } finally {
        Pop-Location
        Remove-Item Env:PG_PORT -ErrorAction SilentlyContinue
        Remove-Item Env:EMQX_DASHBOARD_PORT -ErrorAction SilentlyContinue
        Remove-Item Env:E2E_LOG -ErrorAction SilentlyContinue
    }
    Write-Host "[e2e] 完整输出已落到 $log"
    if ($code -eq 0) {
        Write-Host "[e2e] 全 PASS —— SaaS 控制台端到端冒烟通过。"
    } else {
        Write-Warning "[e2e] 存在 $code 项失败（退出码 $code）。请按日志中的 [saas] FAIL 行逐项排查后再开发。"
    }
}

Ensure-Postgres
Ensure-Emqx

Write-Host ""
Write-Host "=== 容器状态 ==="
wslc ps -a
Write-Host ""
Write-Host "=== 端口映射（宿主 → 容器）==="
Write-Host "  PostgreSQL  : $PG_HOST_PORT → 5432"
Write-Host "  EMQX MQTT   : $($HostFix['Mqtt']) → 1883"
Write-Host "  EMQX WS/WSS : $($HostFix['Ws']) → 8083   /   $($HostFix['Wss']) → 8084"
Write-Host "  EMQX TLS    : $($HostFix['Tls']) → 8883"
Write-Host "  EMQX 控制台 : $($HostFix['Dashboard']) → 18083"

if ($SkipE2E) {
    Write-Host ""
    Write-Host "[e2e] 已按 -SkipE2E 跳过一键自检。"
} else {
    Invoke-E2EGate
}

Write-Host ""
Write-Host "下一步："
Write-Host "  1) 启动后端：dotnet run --project src/plc-saas/plc-saas.csproj   (EnsureCreated + rls.sql 自动建表/RLS)"
Write-Host "     连接串默认已对齐容器：Host=localhost;Port=$PG_HOST_PORT;Database=$($Infra.PgDb);Username=$($Infra.PgAppUser);Password=$($Infra.PgAppPwd)"
Write-Host "     MQTT 桥：Mqtt__BrokerUrl=mqtt://localhost:$($HostFix['Mqtt'])   （路径②需 Mqtt__Bridge__Enabled=true）"
Write-Host "     （宿主端口若非 5432，用 ConnectionStrings__Default 环境变量覆盖 Port）"
Write-Host "  2) 健康检查：curl http://localhost:5000/api/v1/ops/health"
Write-Host "  3) EMQX 控制台：http://localhost:$($HostFix['Dashboard'])  ($($Infra.EmqxUser) / $($Infra.EmqxPwd))"
Write-Host "  4) EMQX 桥接实测：bash scripts/verify-emqx-e2e.sh"
Write-Host "  5) 控制台门禁（本次已跑，需单独复跑时）：bash scripts/verify-saas-console.sh"
