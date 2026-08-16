# =============================================================================
# start-dev-env.ps1  —  本地开发环境启动（WSLC 容器，不用 Docker）
# -----------------------------------------------------------------------------
# 用 wslc（WSL 预览版容器 CLI）拉起 PostgreSQL + EMQX 两个本地容器，
# 供 plc-saas 后端运行期验证（health 端点 + 核心多租户流程 + V1-now MQTT 桥）。
#
# 凭据（按总监裁定）：
#   PostgreSQL : localhost:5432  postgres / postgres   （库 plc_aiot_saas）
#   EMQX       : admin / Admin123  （MQTT 1883 · 控制台 18083 · WS 8083/8084）
#
# 用法：在仓库根目录执行
#   pwsh plc-saas/start-dev-env.ps1
# 停止：
#   wslc stop plc-saas-postgres plc-saas-emqx
# =============================================================================

$ErrorActionPreference = 'Continue'

$PG_NAME    = 'plc-saas-postgres'
$EMQX_NAME  = 'plc-saas-emqx'
$PG_IMAGE   = 'postgres:16'
$EMQX_IMAGE = 'emqx/emqx:5.8.1'
$PG_VOLUME  = 'plc-saas-pgdata'

function Container-Exists($name) {
    $out = wslc ps -a 2>$null
    return ($out -match [regex]::Escape($name))
}
function Container-Running($name) {
    $out = wslc ps 2>$null
    return ($out -match [regex]::Escape($name))
}

function Ensure-Postgres {
    if (Container-Running $PG_NAME) { Write-Host "[pg] 已在运行，跳过"; return }
    if (Container-Exists $PG_NAME) { Write-Host "[pg] 已存在，启动中…"; wslc start $PG_NAME; return }

    Write-Host "[pg] 拉取镜像 $PG_IMAGE …"
    wslc pull $PG_IMAGE 2>&1 | Out-String -Width 200 | Write-Host

    # 持久化数据卷（best-effort，已存在则忽略报错）
    wslc volume create $PG_VOLUME 2>$null

    Write-Host "[pg] 启动容器（POSTGRES_DB 自动建库 plc_aiot_saas）…"
    wslc run -d --name $PG_NAME `
        --volume "${PG_VOLUME}:/var/lib/postgresql/data" `
        -e POSTGRES_USER=postgres `
        -e POSTGRES_PASSWORD=postgres `
        -e POSTGRES_DB=plc_aiot_saas `
        -p 5432:5432 `
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
