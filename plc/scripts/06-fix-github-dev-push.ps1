#Requires -Version 5.1
<#
.SYNOPSIS
    修复 Rider/IntelliJ「提交到 dev 分支失败: Software caused connection abort」
    根因：IDE 在推送前自动执行 `git fetch --no-tags origin`，
          由于 libcurl 没有通过本机代理访问 GitHub（直连 443 TCP 被断），
          导致 fetch 阶段 abort，整个「提交并同步」被标记为失败。

.DESCRIPTION
    1) 检测本机是否存在代理（默认 Clash / v2rayN 端口 7890；可通过 -Proxy 覆盖）。
    2) 把代理写入仓库级 git config（http.proxy / https.proxy），
       并配置 HTTP/1.1、postBuffer、GCM 凭据助手，
       从而让 Rider/IntelliJ、Git For Windows 统一走代理 CONNECT。
    3) 探测 https://github.com (通过代理) 连通性；
    4) 执行一次 ls-remote + fetch（no-tags）+ push dry-run，
       全部成功则提示「下一次 IDE 的 Commit and Push 就不会再报错」。
    5) 若需要 PAT（不想每次弹凭据），提供交互式写入 https://user:token@... remote URL 功能。

.PARAMETER Proxy
    代理地址，如 'http://127.0.0.1:7890'。默认自动探测。
.PARAMETER RemoveProxy
    切换回无代理环境时使用：移除本仓库 proxy 配置，其它调优保留。
.PARAMETER SetPatRemote
    提供时提示输入 UserName + FineGrainedPAT 并覆盖 origin 为带 token 的 HTTPS URL。
.EXAMPLE
    .\06-fix-github-dev-push.ps1
    # 自动使用默认 7890 端口代理
.EXAMPLE
    .\06-fix-github-dev-push.ps1 -Proxy 'http://127.0.0.1:10809'
    # 指定自定义代理端口（比如 v2rayN 的 HTTP 10809）
.EXAMPLE
    .\06-fix-github-dev-push.ps1 -RemoveProxy
    # 清除代理配置
#>
[CmdletBinding(DefaultParameterSetName = 'Setup')]
param(
    [Parameter(ParameterSetName = 'Setup')]
    [string]$Proxy = '',

    [Parameter(ParameterSetName = 'Remove')]
    [switch]$RemoveProxy,

    [Parameter(ParameterSetName = 'Setup')]
    [switch]$SetPatRemote
)

$ErrorActionPreference = 'Stop'

# —— 0. 定位仓库根 ——
$RepoRoot = Split-Path -Parent $PSScriptRoot   # plc/scripts -> plc/
Write-Host "`n=== PlcVsa dev 提交网络修复 (Windows PowerShell) ===" -ForegroundColor Cyan
Write-Host "RepoRoot = $RepoRoot"

Push-Location $RepoRoot
try {
    git rev-parse --show-toplevel 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) { throw '当前目录不是一个 git 仓库。' }

    # —— 1. 确定代理 ——
    if ($RemoveProxy) {
        Write-Host "`n[1/6] 移除仓库级 proxy 配置…" -ForegroundColor Yellow
        @('http.proxy','https.proxy') | ForEach-Object {
            git config --local --unset-all $_ 2>$null
        }
        Write-Host "   已移除 http.proxy / https.proxy"
    } else {
        if ([string]::IsNullOrWhiteSpace($Proxy)) {
            # 常见 Windows 本机代理端口：Clash=7890, v2rayN=10809, 系统代理(IE) 需另行读
            $candidates = @(
                'http://127.0.0.1:7890',
                'http://127.0.0.1:10809',
                'http://127.0.0.1:1080',
                'http://127.0.0.1:18080'
            )
            foreach ($c in $candidates) {
                $m = [regex]::Match($c, '^http://([^:]+):(\d+)$')
                if ($m.Success) {
                    $host_ = $m.Groups[1].Value
                    $port  = [int]$m.Groups[2].Value
                    $tcp = New-Object System.Net.Sockets.TcpClient
                    try {
                        $iasync = $tcp.BeginConnect($host_, $port, $null, $null)
                        if ($iasync.AsyncWaitHandle.WaitOne(200)) { $tcp.EndConnect($iasync); $Proxy = $c; break }
                    } catch {} finally { try { $tcp.Close() } catch {} }
                }
            }
            if ([string]::IsNullOrWhiteSpace($Proxy)) {
                Write-Warning "未在常见端口探测到本地代理 (7890/10809/1080/18080)。请使用 -Proxy 指定，比如:"
                Write-Warning "  .\06-fix-github-dev-push.ps1 -Proxy 'http://127.0.0.1:端口号'"
                exit 2
            }
            Write-Host "`n[1/6] 自动探测代理: $Proxy" -ForegroundColor Cyan
        } else {
            Write-Host "`n[1/6] 使用传入代理: $Proxy" -ForegroundColor Cyan
        }

        Write-Host "[2/6] 写入 git config（仓库级，不影响其他仓库）…"
        git config --local http.proxy  $Proxy
        git config --local https.proxy $Proxy
        git config --local http.version     HTTP/1.1
        git config --local http.sslVerify  true
        git config --local http.postBuffer  524288000
        git config --local http.lowSpeedLimit 0
        git config --local http.lowSpeedTime  999999
        # 凭据助手优先用 Git Credential Manager（随 Git For Windows 自带）
        try {
            $helper = git config --global credential.helper 2>$null
            if ([string]::IsNullOrWhiteSpace($helper)) {
                git config --global credential.helper manager-core 2>$null
                git config --global credential.https://github.com.provider github 2>$null
            }
        } catch {}
    }

    Write-Host "`n--- 仓库级当前 http(s) 配置 ---"
    git config --local --list | Select-String '^(http|https|credential)\.'

    # —— 3. HTTPS 代理连通性探测 ——
    Write-Host "`n[3/6] 探测 GitHub HTTPS (经代理)…"
    try {
        $target = 'https://github.com/windgodsnowdrink/vsa.git/info/refs?service=git-upload-pack'
        $resp = Invoke-WebRequest -Uri $target -Method Head -UseBasicParsing `
                                  -Proxy $Proxy -ProxyUseDefaultCredentials:$false `
                                  -TimeoutSec 15 -ErrorAction Stop
        Write-Host "   OK: HTTP $($resp.StatusCode) ($([math]::Round($(Get-Date).Ticks/1e4)) ms-ish)"
    } catch {
        Write-Warning "代理访问 GitHub 失败：$($_.Exception.Message)"
        Write-Warning "如果你的代理需要用户名/密码，写成： -Proxy 'http://user:pass@host:port'"
        exit 3
    }

    # —— 4. ls-remote ——
    Write-Host "`n[4/6] git ls-remote origin (15s timeout)…"
    $job = Start-Job -ScriptBlock {
        param($Root)
        Set-Location $Root
        git ls-remote --symref origin HEAD refs/heads/dev refs/heads/main 2>&1
    } -ArgumentList $RepoRoot
    if (Wait-Job $job -Timeout 15) {
        $out = Receive-Job $job
        Remove-Job $job -Force
        $out | ForEach-Object { Write-Host "   $_" }
        if ($LASTEXITCODE -ne 0 -and -not [string]::IsNullOrWhiteSpace($out)) {
            # ls-remote 在 job 里 LASTEXITCODE 不一定准，输出里是否出现 fatal?
            if ($out -match 'fatal:') { throw "ls-remote 失败" }
        }
    } else {
        Remove-Job $job -Force
        throw "ls-remote 15 秒超时，可能代理不稳定或代理无法 CONNECT github.com:443。"
    }

    # —— 5. fetch（重现用户失败那一条）——
    Write-Host "`n[5/6] git fetch --no-tags origin （重现 IDE pre-push fetch）…"
    & git -c diff.mnemonicprefix=false -c core.quotepath=false --no-optional-locks fetch --no-tags origin 2>&1 | ForEach-Object { Write-Host "   $_" }
    if ($LASTEXITCODE -ne 0) { throw "fetch 仍然失败，exit=$LASTEXITCODE" }
    Write-Host "   OK (exit=0) —— IDE 「提交前 fetch」不再会报 Software caused connection abort。"

    # —— 6. push dry-run + (可选) PAT remote URL ——
    Write-Host "`n[6/6] push --dry-run origin dev…"
    $remoteUrl = git remote get-url origin
    $needCred = $false
    & git push --dry-run --verbose origin dev 2>&1 | ForEach-Object {
        Write-Host "   $_"
        if ($_ -match 'could not read Username') { $script:needCred = $true }
    }

    if ($needCred -or $SetPatRemote) {
        Write-Host "`n   检测到 HTTPS 方式需要凭据 / 或你指定了 -SetPatRemote。" -ForegroundColor Yellow
        Write-Host "   推荐在 GitHub 上生成 Fine-grained Personal Access Token:"
        Write-Host "     Settings -> Developer settings -> Personal access tokens -> Fine-grained tokens"
        Write-Host "     -> Repository access: Only select repositories -> 选 windgodsnowdrink/vsa"
        Write-Host "     -> Permissions: Contents (Read and write), Metadata (Read-only)"
        if ($SetPatRemote) {
            $user   = Read-Host "GitHub Username"
            $token  = Read-Host "GitHub Fine-grained PAT (粘贴后回车)" -AsSecureString
            $plain  = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
                         [Runtime.InteropServices.Marshal]::SecureStringToBSTR($token))
            $newUrl = "https://${user}:$plain@github.com/windgodsnowdrink/vsa.git"
            git remote set-url origin $newUrl
            Write-Host "   已写入带 PAT 的 remote URL（仅写入当前仓库级 .git/config）。" -ForegroundColor Green
            & git push --verbose --porcelain origin dev 2>&1 | ForEach-Object { Write-Host "   $_" }
        } else {
            Write-Host "   本次不写入 PAT。后续你可以："
            Write-Host "    (A) 重新使用 Rider 的 Commit and Push，它会弹出 Git Credential Manager 登录框；"
            Write-Host "    (B) 或重跑： .\06-fix-github-dev-push.ps1 -SetPatRemote"
        }
    }

    # —— 收尾：dev 分支一致性 ——
    Write-Host "`n=== 最终状态 ===" -ForegroundColor Cyan
    git status -sb
    $HEAD = (git rev-parse --short=10 HEAD)
    $ORIG = (git rev-parse --short=10 refs/remotes/origin/dev 2>$null)
    Write-Host "HEAD       = $HEAD"
    Write-Host "origin/dev = $(if($ORIG){$ORIG}else{'MISSING'})"
    if ($HEAD -eq $ORIG) {
        Write-Host "OK: 本地 dev 与 origin/dev 提交一致，无需再次 push。" -ForegroundColor Green
    } else {
        Write-Warning "存在待 push 的本地提交，上面的 dry-run 若未报 fatal，执行：git push origin dev 即可。"
    }

} finally {
    Pop-Location
}
Write-Host "`n完成。如之后 Rider / IntelliJ 仍偶发 fetch abort，请重启 IDE 使新版 git config 生效。`n" -ForegroundColor Cyan
