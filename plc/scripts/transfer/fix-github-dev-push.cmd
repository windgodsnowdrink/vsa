@echo off
REM =====================================================================
REM  PlcVsa · dev 分支：修复 JetBrains「提交失败 Network abort」一键脚本 (CMD 版)
REM  双击即可运行（比 PowerShell 更容易被老 Windows 直接跑）
REM  用法：
REM    1) 打开命令行 cmd.exe 到 plc 仓库根：
REM         cd /d E:\WorkSpace\windgodsnowdrink\vsa\plc
REM    2) scripts\transfer\fix-github-dev-push.cmd
REM    3) 如果需要使用 Fine-grained PAT 避免 GCM 弹窗，第一次加参数：
REM         scripts\transfer\fix-github-dev-push.cmd WITH_PAT
REM       脚本会依次提示你输入 GitHub Username 与 Fine-grained PAT
REM =====================================================================

setlocal enabledelayedexpansion enableextensions
cd /d "%~dp0\..\.."
echo.
echo === PlcVsa: fix GitHub dev push ======================================
echo [RepoRoot] %cd%
if not exist .git (
  echo ERROR: 当前目录不是 git 仓库，请把脚本复制到 plc\scripts\transfer\ 下再运行。
  exit /b 2
)

where git >nul 2>nul
if errorlevel 1 (
  echo ERROR: 找不到 git.exe，请先安装 Git For Windows：https://git-scm.com/download/win
  exit /b 3
)

REM —— (1) 探测本机代理端口：7890 (Clash) / 10809 (v2rayN HTTP) / 1080 ——
set PROXY=
for %%p in (7890 10809 1080 18080) do (
  powershell -NoProfile -Command "try{$c=New-Object Net.Sockets.TcpClient;$c.Connect('127.0.0.1',%%p);$c.Close();exit 0}catch{exit 1}"
  if !errorlevel!==0 set PROXY=http://127.0.0.1:%%p&& goto :proxy_found
)
:proxy_found
if "%PROXY%"=="" (
  echo.
  echo WARN: 没探测到 Clash(7890)/v2rayN(10809)/SOCKS(1080)/沙箱(18080) 代理
  echo       如果不使用代理，请按 Ctrl-C 退出；否则继续会把代理置空写入 config.
  pause
) else (
  echo [AutoDetect Proxy] %PROXY%
)

REM —— (2) 写仓库级 git config ——
echo.
echo [Step 1/5] 写入 git config（仅当前仓库，不影响全局）
if not "%PROXY%"=="" (
  git config --local http.proxy  %PROXY%
  git config --local https.proxy %PROXY%
)
git config --local http.version    HTTP/1.1
git config --local http.sslVerify  true
git config --local http.postBuffer 524288000
git config --global credential.helper manager-core 2>nul
git config --global credential.https://github.com.provider github 2>nul

echo --- 当前 repository http(s).proxy 配置 ---
git config --local --list | findstr /i /r "^http\. ^https\. ^remote\.origin\.url"

REM —— (3) HTTPS 代理探测 ——
echo.
echo [Step 2/5] 探测 GitHub 代理 CONNECT （30s 超时）
powershell -NoProfile -Command "$ProgressPreference='SilentlyContinue'; try{ $p='%PROXY%'; if ([string]::IsNullOrWhiteSpace($p)) { Invoke-WebRequest -UseBasicParsing -Uri 'https://github.com' -TimeoutSec 30 | Out-Null } else { Invoke-WebRequest -UseBasicParsing -Proxy $p -Uri 'https://github.com' -TimeoutSec 30 | Out-Null }; Write-Host '  OK: HTTP CONNECT to https://github.com via proxy' } catch { Write-Host ('  FAIL: ' + $_.Exception.Message); exit 4 }"
if errorlevel 1 (
  echo 代理访问 GitHub 仍然失败，请检查代理端口是否正确；或显式指定：
  echo    git config --local http.proxy http://127.0.0.1:端口号
  pause & exit /b 4
)

REM —— (4) ls-remote + fetch（原失败命令）——
echo.
echo [Step 3/5] git ls-remote origin dev main HEAD
git ls-remote --symref origin HEAD refs/heads/dev refs/heads/main
if errorlevel 1 goto :end

echo.
echo [Step 4/5] 重放原来报 abort 的 fetch --no-tags origin
git -c diff.mnemonicprefix=false -c core.quotepath=false --no-optional-locks fetch --no-tags origin
if errorlevel 1 goto :end
echo   FETCH exit=0  -- Software caused connection abort 已修复 ✓

REM —— (5) 可选写 PAT + push ——
echo.
if /I "%~1"=="WITH_PAT" goto :ask_pat
echo [Step 5/5] push --dry-run origin dev（不真正推送）
git push --dry-run --verbose origin dev
goto :summary

:ask_pat
echo [Step 5/5] 写入 Fine-grained PAT 并真正 push origin dev
set /P GHUSER=GitHub Username:
set /P GHPAT=GitHub Fine-grained PAT:
if "%GHUSER%"=="" echo Username 为空 & goto :end
if "%GHPAT%"==""  echo PAT 为空      & goto :end
git remote set-url origin "https://%GHUSER%:%GHPAT%@github.com/windgodsnowdrink/vsa.git"
git push --verbose --porcelain origin dev

:summary
echo.
echo === 完成 ============================================================
echo  当前分支 ：
git status -sb
echo.
echo  如果 Rider/IntelliJ 仍偶发 fetch abort，请重启 IDE 让新 git config 生效。
echo  Windows 端其他可用命令（粘到 PowerShell）：
echo    cd /d E:\WorkSpace\windgodsnowdrink\vsa\plc
echo    powershell -ExecutionPolicy Bypass -File .\scripts\06-fix-github-dev-push.ps1
echo    git push origin dev
:end
endlocal
if not "%RUNNER_TEMP%"=="" exit /b %errorlevel%
pause
