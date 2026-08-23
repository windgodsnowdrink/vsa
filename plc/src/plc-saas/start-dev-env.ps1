# =============================================================================
# start-dev-env.ps1  —  本地开发环境启动（WSLC 容器，不用 Docker）
# -----------------------------------------------------------------------------
# 用 wslc（WSL 预览版容器 CLI）拉起 EMQX 一个本地容器，
# 供 plc-saas 后端运行期验证（health 端点 + 核心多租户流程 + V1-now MQTT 桥）。
# 镜像源：Docker Hub(registry-1.docker.io) 在本机/沙箱不可达，统一改用 docker.1ms.run 镜像加速器。
#
# 数据库：本地 SQL Server（Windows 账户登录，Integrated Security=True）。
#   不再起 PostgreSQL 容器；appsettings 连接串 Server=localhost;Integrated Security=True;Encrypt=False。
#   需本机已安装 SQL Server 默认实例，且当前 Windows 账户有建库/建表权限。
#   首次启动由 EnsureCreated 幂等建库表 + RLS（sql/rls.sql）。
#
# 凭据（按总监裁定）：
#   EMQX       : admin / Admin123  （MQTT 1883 · 控制台 18083 · WS 8083/8084）
#
# 用法：在仓库根目录执行
#   pwsh plc-saas/start-dev-env.ps1
# 停止：
#   wslc stop plc-saas-emqx
# =============================================================================

$ErrorActionPreference = 'Stop'

$EMQX_NAME  = 'plc-saas-emqx'
$EMQX_IMAGE = 'docker.1ms.run/emqx/emqx:6.1.4'                # docker.1ms.run 镜像源（EMQX 6.x）

function Container-Exists($name) {
    $out = wslc ps -a 2>$null
    return ($out -match [regex]::Escape($name))
}
function Container-Running($name) {
    $out = wslc ps 2>$null
    return ($out -match [regex]::Escape($name))
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

Ensure-Emqx

Write-Host ""
Write-Host "=== 容器状态 ==="
wslc ps -a
Write-Host ""
Write-Host "数据库：本地 SQL Server（Windows 账户登录），EnsureCreated 首启自动建库表 + RLS。"
Write-Host "下一步："
Write-Host "  1) 启动后端：dotnet run --project plc-saas/plc-saas.csproj   (EnsureCreated + rls.sql 自动建表/RLS)"
Write-Host "  2) 健康检查：curl -u <admin-token> http://localhost:5000/api/v1/ops/health"
Write-Host "  3) EMQX 控制台：http://localhost:18083  (admin / Admin123)"
