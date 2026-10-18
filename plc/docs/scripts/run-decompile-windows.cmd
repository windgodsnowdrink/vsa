@echo off
REM =====================================================================
REM  一键反编译 snet DLL（Windows）
REM  用法：
REM    1) 把本文件放在 plc\docs\scripts\ 目录下（已在该目录）
REM    2) 双击 run-decompile-windows.cmd
REM    3) 等待 dnSpy / ilspycmd 双路线反编译完成
REM
REM  前置条件（在 Windows 本机）：
REM    - DLL 目录: E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet\
REM    - .NET SDK 已安装 (dotnet --version 可用) —— ilspycmd 路线需要
REM    - 可选: E:\Net Tool\dnSpy\ 目录 (dnSpy 路线, 没有则自动走 ilspycmd)
REM
REM  产物:
REM    E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\src\
REM    —— 每个包一个子目录，含反编译出的 .cs 源文件
REM =====================================================================
setlocal enabledelayedexpansion

REM —— 进入本 .cmd 所在目录 (docs\scripts) ——
cd /d "%~dp0"
echo.
echo === PlcVsa snet DLL 一键反编译 =====================================
echo [Repo scripts dir] %cd%
echo.

REM —— 0. 检查 git 是否可用 ——
where git >nul 2>nul
if errorlevel 1 (
  echo ERROR: 找不到 git.exe，请先安装 Git For Windows。
  pause & exit /b 1
)

REM —— 1. 拉取最新脚本（避免本地脚本过时）——
echo [Step 1/4] git pull origin dev  (确保拿到最新 05-decompile-snet-fallback.ps1)
git pull origin dev
if errorlevel 1 (
  echo WARN: git pull 失败（可能无网络或冲突），继续使用本地已有脚本...
)
echo.

REM —— 2. 检查核心脚本是否存在 ——
set SCRIPT=05-decompile-snet-fallback.ps1
if not exist "%SCRIPT%" (
  echo ERROR: 找不到 %SCRIPT%，请确认本 .cmd 放在 plc\docs\scripts\ 目录下。
  echo 当前目录: %cd%
  dir /b *.ps1
  pause & exit /b 2
)
echo [Step 2/4] 找到反编译脚本: %SCRIPT%
echo.

REM —— 3. 检查 dotnet (ilspycmd 路线必需) ——
echo [Step 3/4] 检查 .NET SDK ...
where dotnet >nul 2>nul
if errorlevel 1 (
  echo WARN: 未找到 dotnet，ilspycmd 回退路线不可用。
  echo       请安装 .NET SDK (https://dotnet.microsoft.com/download)
  echo       若 E:\Net Tool\dnSpy\ 存在，仍可走 dnSpy 路线。
) else (
  for /f "delims=" %%v in ('dotnet --version') do echo   dotnet %%v  OK
)
echo.

REM —— 4. 执行反编译（PowerShell ExecutionPolicy Bypass）——
echo [Step 4/4] 执行 05-decompile-snet-fallback.ps1 ...
echo.
powershell -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT%"
set EXITCODE=%errorlevel%
echo.
echo === 反编译完成 (exit=%EXITCODE%) ====================================
echo.
if "%EXITCODE%"=="0" (
  echo   成功！产物在:
  echo     E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\src\
  echo   报告:
  echo     E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\decompile-report.csv
) else (
  echo   部分 DLL 反编译失败，请查看上方日志和报告 CSV。
  echo   把失败行贴回来可继续处理。
)
echo.
echo 按任意键退出...
pause >nul
endlocal
exit /b %EXITCODE%
