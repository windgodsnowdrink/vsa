<#
.SYNOPSIS
  反编译 snet 目录下所有 DLL：
    第一路线  → E:\Net Tool\dnSpy\  (dnSpy.Console / ICSharpCode.Decompiler)
    对 dnSpy 失败的每个 DLL  → 第二路线自动回退 ilspycmd (ICSharpCode.ILSpyCmd dotnet tool)
  两路产物分桶存放，最后汇总为统一目录 snet-decompiled\src\（去重 + 优先级 dnSpy>ilspycmd）。
.PREREQ
  - PowerShell 5.1+ / 7+
  - .NET SDK (8.0/9.0/10.0/11.0 任一可用 dotnet 命令，ilspycmd 路线用)
  - dnSpy 目录: E:\Net Tool\dnSpy\  (优先)
  - snet DLL 根: E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet\
.NOTES
  产物目录结构：
    snet-decompiled/
      inventory/            → 01 脚本产物（DLL 清单 / 引用表）
      src/                  → 统一汇总（每包一个目录，优先 dnSpy 结果）
      _per_tool/
        dnspy/<pkg>/        → dnSpy 路线原始输出
        ilspy/<pkg>/        → ilspycmd 路线原始输出（仅 dnSpy 失败时才出现）
      decompile-report.csv  → 每个 DLL 走了哪条路线 / 成功 / 失败原因 / 解出 .cs 数量
#>

param(
  [string]$SnetRoot     = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet",
  [string]$DnSpyDir     = "E:\Net Tool\dnSpy",
  [string]$OutDir       = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled",
  [string]$ILSpyCmdVer  = "8.3.0.7756"
)

$ErrorActionPreference = "Continue"

# =============== 0) 目录 + log ===============
New-Item -ItemType Directory -Force -Path $OutDir                         | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $OutDir "inventory") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $OutDir "src")       | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $OutDir "_per_tool\dnspy") | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $OutDir "_per_tool\ilspy") | Out-Null

$ReportCsv  = Join-Path $OutDir "decompile-report.csv"
$LogFile    = Join-Path $OutDir "decompile.log"
Clear-Content -LiteralPath $LogFile   -ErrorAction SilentlyContinue
Clear-Content -LiteralPath $ReportCsv -ErrorAction SilentlyContinue

function Write-Log([string]$m, [string]$Level="INFO") {
  $line = (Get-Date -Format 'yyyy-MM-dd HH:mm:ss') + "  [$Level]  $m"
  Add-Content -LiteralPath $LogFile -Value $line -Encoding UTF8
  switch ($Level) {
    "OK"   { Write-Host $line -ForegroundColor Green }
    "WARN" { Write-Host $line -ForegroundColor Yellow }
    "ERR"  { Write-Host $line -ForegroundColor Red }
    default { Write-Host $line }
  }
}

# =============== 1) 盘点 DLL (复用 01) ===============
Write-Log "Step 1: 盘点 snet DLL 清单 ..."
$scriptsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$invScript  = Join-Path $scriptsDir "01-inventory-snet.ps1"
$invCsv     = Join-Path $OutDir "inventory\snet-dll-inventory.csv"
if (-not (Test-Path -LiteralPath $invScript)) {
  throw "[FATAL] 找不到 01-inventory-snet.ps1：$invScript"
}
& $invScript -SnetRoot $SnetRoot -OutDir (Join-Path $OutDir "inventory")
$inv = @()
if (Test-Path -LiteralPath $invCsv) { $inv = Import-Csv -LiteralPath $invCsv }
if (-not $inv -or $inv.Count -eq 0) {
  throw "[FATAL] 盘点产物为空，无法继续：$invCsv"
}
Write-Log "Step 1 完成：$($inv.Count) 个 DLL (来自 $($inv | Group-Object Package | Measure-Object).Count 个包)"

# =============== 2) 准备工具链 ===============
Write-Log "Step 2: 准备工具链 ..."

# --- 2a) dnSpy 能力探测 ---
$Mode_dnSpy   = $null
$dnConsoleExe = Join-Path $DnSpyDir "dnSpy.Console.exe"
$dnConsoleDll = Join-Path $DnSpyDir "dnSpy.Console.dll"
$decDll       = Join-Path $DnSpyDir "ICSharpCode.Decompiler.dll"
if (Test-Path -LiteralPath $dnConsoleExe) { $Mode_dnSpy = "Exe" }
elseif (Test-Path -LiteralPath $dnConsoleDll) { $Mode_dnSpy = "Dll" }
elseif (Test-Path -LiteralPath $decDll)       { $Mode_dnSpy = "API" }
else {
  Write-Log "dnSpy 目录内无可执行/可引用反编译核心，将跳过 dnSpy 路线并全部走 ilspycmd" "WARN"
  $Mode_dnSpy = $null
}

# --- 2b) ilspycmd 能力准备（一定装，即使仅作 fallback） ---
$ilspyOk = $false
try {
  $null = Get-Command dotnet -ErrorAction Stop
  if (-not (Test-Path (Join-Path $scriptsDir ".config\dotnet-tools.json"))) {
    Push-Location $scriptsDir
    dotnet new tool-manifest | Out-Null
    Pop-Location
  }
  Push-Location $scriptsDir
  $installStderr = (dotnet tool install ICSharpCode.ILSpyCmd --version $ILSpyCmdVer 2>&1)
  # 如果已装但版本不对 -> 先 uninstall 再 install
  if ($LASTEXITCODE -ne 0 -and ($installStderr -join "`n") -match "already installed|ToolManifest|is already installed") {
    dotnet tool uninstall ICSharpCode.ILSpyCmd 2>&1 | Out-Null
    dotnet tool install   ICSharpCode.ILSpyCmd --version $ILSpyCmdVer 2>&1 | Out-Null
  }
  Pop-Location
  # 验证
  Push-Location $scriptsDir
  $vTest = (dotnet tool run ilspycmd -- --version 2>&1) -join " "
  Pop-Location
  if ($LASTEXITCODE -eq 0 -or $vTest -match "^\d+\.\d+") {
    $ilspyOk = $true
    Write-Log "ilspycmd 可用: $vTest"
  } else {
    Write-Log "ilspycmd 安装/验证失败（exit=$LASTEXITCODE），ilspy 回退路线不可用：$vTest" "WARN"
  }
} catch {
  Write-Log "dotnet 不可用或 ilspycmd 安装异常：$($_.Exception.Message)，ilspy 路线不可用" "WARN"
}

if (-not $Mode_dnSpy -and -not $ilspyOk) {
  throw "[FATAL] 两条路线都不可用：dnSpy 目录无效 + ilspycmd 装不上。请手动检查。"
}

# =============== 3) 逐 DLL 反编译 ===============
Write-Log "Step 3: 逐 DLL 反编译 (先 dnSpy，失败再 ilspycmd) ..."
$reports = New-Object System.Collections.Generic.List[object]

function Invoke-DnSpy($pkg, $dllPath, $outPkg) {
  if (-not $Mode_dnSpy) { return @{Ok=$false; Reason="dnSpy-not-available"} }
  $errLog = Join-Path $outPkg "_dnspy-err.log"
  $outLog = Join-Path $outPkg "_dnspy-out.log"
  try {
    switch ($Mode_dnSpy) {
      "Exe" {
        $p = Start-Process -FilePath $dnConsoleExe -ArgumentList @(
            $dllPath, "-o", $outPkg, "--single-types", "--resolve-path", $SnetRoot
          ) -NoNewWindow -Wait -PassThru -RedirectStandardError $errLog -RedirectStandardOutput $outLog
        if ($p.ExitCode -ne 0) { return @{Ok=$false; Reason="exit=$($p.ExitCode)"; Log=$errLog} }
      }
      "Dll" {
        $p = Start-Process -FilePath dotnet -ArgumentList @(
            $dnConsoleDll, $dllPath, "-o", $outPkg, "--single-types", "--resolve-path", $SnetRoot
          ) -NoNewWindow -Wait -PassThru -RedirectStandardError $errLog -RedirectStandardOutput $outLog
        if ($p.ExitCode -ne 0) { return @{Ok=$false; Reason="exit=$($p.ExitCode)"; Log=$errLog} }
      }
      "API" {
        # 用内联 C# 脚本 + 反编译 DLL 反射实现
        $driver = Join-Path $outPkg "_driver.cs"
        $code = @"
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

class Driver {
  static int Main(string[] args) {
    var dll  = args[0];
    var oDir = args[1];
    var root = args[2];
    Directory.CreateDirectory(oDir);
    var resolver = new UniversalAssemblyResolver(dll, false, null);
    foreach (var d in Directory.GetDirectories(root, "lib", SearchOption.AllDirectories))
      try { resolver.AddSearchDirectory(d); } catch {}
    var settings = new DecompilerSettings(LanguageVersion.Latest) {
        ThrowOnAssemblyResolveErrors = false,
        UseSdkStyleProjectFormat = true,
        AlwaysUseBraces = true,
        ShowXmlDocumentation = false,
    };
    var dec = new CSharpDecompiler(dll, resolver, settings);
    int count = 0;
    foreach (var t in dec.TypeSystem.MainModule.TypeDefinitions) {
      var ns = string.IsNullOrEmpty(t.Namespace) ? "_global_" : t.Namespace;
      var dir = Path.Combine(oDir, ns.Replace('.', Path.DirectorySeparatorChar));
      Directory.CreateDirectory(dir);
      var name = t.Name;
      // 处理泛型 / 嵌套类里非法文件名
      name = name.Replace('`','_').Replace('<','_').Replace('>','_').Replace('\\','_').Replace('/','_').Replace('|','_').Replace(':','_').Replace('?','_').Replace('*','_').Replace('"','_');
      try {
        var src = dec.DecompileTypeAsString(t.MetadataToken);
        File.WriteAllText(Path.Combine(dir, name + ".cs"), src);
        count++;
      } catch (Exception ex) {
        File.WriteAllText(Path.Combine(dir, name + ".error.txt"), ex.ToString());
      }
    }
    Console.WriteLine("count="+count);
    return 0;
  }
}
"@
        Set-Content -LiteralPath $driver -Value $code -Encoding UTF8
        $proj = Join-Path $outPkg "_dnspy-api.csproj"
        $projXml = @"
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>
  <OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>
  <Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings>
  <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
</PropertyGroup><ItemGroup>
  <Reference Include="ICSharpCode.Decompiler"><HintPath>$decDll</HintPath></Reference>
  <Compile Include="_driver.cs" />
</ItemGroup></Project>
"@
        Set-Content -LiteralPath $proj -Value $projXml -Encoding UTF8
        Push-Location $outPkg
        $p = Start-Process -FilePath dotnet -ArgumentList @(
            "run", "--project", "_dnspy-api.csproj", "--", $dllPath, $outPkg, $SnetRoot
          ) -NoNewWindow -Wait -PassThru -RedirectStandardError $errLog -RedirectStandardOutput $outLog
        Pop-Location
        if ($p.ExitCode -ne 0) { return @{Ok=$false; Reason="api-exit=$($p.ExitCode)"; Log=$errLog} }
      }
    }
    $cs = (Get-ChildItem -LiteralPath $outPkg -Recurse -Filter *.cs -ErrorAction SilentlyContinue | Measure-Object).Count
    return @{Ok=$true; Files=$cs}
  } catch {
    return @{Ok=$false; Reason="exception="+$_.Exception.Message}
  }
}

function Invoke-ILSpyCmd($pkg, $dllPath, $outPkg) {
  if (-not $ilspyOk) { return @{Ok=$false; Reason="ilspycmd-not-available"} }
  $errLog = Join-Path $outPkg "_ilspy-err.log"
  $outLog = Join-Path $outPkg "_ilspy-out.log"
  try {
    Push-Location $scriptsDir
    $p = Start-Process -FilePath dotnet -ArgumentList @(
        "tool","run","ilspycmd",
        "--outputdir", $outPkg,
        "--project-list-style","SingleFilePerType",
        "--no-dead-stores","--remove-vb-style-casts","--inline-casts","--unicode",
        "--search-path", $SnetRoot,
        $dllPath
      ) -NoNewWindow -Wait -PassThru -RedirectStandardError $errLog -RedirectStandardOutput $outLog
    Pop-Location
    if ($p.ExitCode -ne 0) { return @{Ok=$false; Reason="exit=$($p.ExitCode)"; Log=$errLog} }
    $cs = (Get-ChildItem -LiteralPath $outPkg -Recurse -Filter *.cs -ErrorAction SilentlyContinue | Measure-Object).Count
    return @{Ok=$true; Files=$cs}
  } catch {
    Pop-Location
    return @{Ok=$false; Reason="exception="+$_.Exception.Message}
  }
}

$idx = 0; $total = $inv.Count; $dnSpyWin = 0; $ilspyWin = 0; $allFail = 0

foreach ($row in $inv) {
  $idx++
  $pkg     = $row.Package
  $dllPath = $row.Path
  $tfm     = $row.Tfm
  $status  = "FAIL-BOTH"
  $tool    = ""
  $csCount = 0
  $reason  = ""
  $chosen  = $null

  if (-not (Test-Path -LiteralPath $dllPath)) {
    $reason = "dll-missing"; $status = "SKIP"
  }
  else {
    # --- Route 1: dnSpy ---
    $outDn = Join-Path $OutDir "_per_tool\dnspy\$pkg"
    New-Item -ItemType Directory -Force -Path $outDn | Out-Null
    if ($Mode_dnSpy) {
      $r = Invoke-DnSpy $pkg $dllPath $outDn
      if ($r.Ok) {
        $status = "OK"; $tool = "dnSpy($Mode_dnSpy)"; $csCount = $r.Files; $chosen = $outDn; $dnSpyWin++
        Write-Log "[$idx/$total] dnSpy OK   $pkg ($csCount cs)" "OK"
      } else {
        $reason = "dnSpy:" + $r.Reason
        Write-Log "[$idx/$total] dnSpy FAIL $pkg -> $($r.Reason)  → fallback ilspycmd" "WARN"
      }
    } else {
      $reason = "dnSpy:skipped-unavailable"
    }

    # --- Route 2: ilspycmd fallback (if dnSpy failed/unavailable) ---
    if (-not $chosen) {
      $outIL = Join-Path $OutDir "_per_tool\ilspy\$pkg"
      New-Item -ItemType Directory -Force -Path $outIL | Out-Null
      $r2 = Invoke-ILSpyCmd $pkg $dllPath $outIL
      if ($r2.Ok) {
        $status = "OK"; $tool = "ilspycmd"; $csCount = $r2.Files; $chosen = $outIL; $ilspyWin++
        Write-Log "[$idx/$total] ilspycmd OK (fallback) $pkg ($csCount cs)" "OK"
      } else {
        $allFail++
        if ($reason) { $reason += " | " }
        $reason += "ilspycmd:" + $r2.Reason
        Write-Log "[$idx/$total] BOTH FAIL $pkg : $reason" "ERR"
      }
    }
  }

  # --- copy winner to unified src ---
  $unified = Join-Path $OutDir "src\$pkg"
  if ($chosen) {
    if (Test-Path -LiteralPath $unified) { Remove-Item -LiteralPath $unified -Recurse -Force }
    Copy-Item -LiteralPath $chosen -Destination $unified -Recurse
    # 清理过程日志/驱动文件（保留原始工具日志在 _per_tool 里）
    Get-ChildItem -LiteralPath $unified -Recurse -File -Include "_*.log","_*.csx","_*.cs","_*.csproj","_*.error.txt" |
      Remove-Item -Force -ErrorAction SilentlyContinue
  }

  $reports.Add([pscustomobject]@{
    Idx        = $idx
    Package    = $pkg
    Tfm        = $tfm
    DllPath    = $dllPath
    Status     = $status
    Tool       = $tool
    CsFiles    = $csCount
    FailReason = $reason
    UnifiedDir = if ($chosen) { $unified } else { "" }
  })
}

# =============== 4) 汇总 ===============
$reports | Export-Csv -NoTypeInformation -Encoding UTF8 -LiteralPath $ReportCsv

$summary = @"
==================== snet 反编译汇总 ====================
Total DLLs           : $total
  dnSpy 路线成功      : $dnSpyWin
  ilspycmd 回退成功   : $ilspyWin
  双路线全部失败      : $allFail
  SKIP(缺 DLL)        : $($reports.Where({$_.Status -eq 'SKIP'}).Count)
---------------------------------------------------------
统一源码目录         : $(Join-Path $OutDir "src")
逐工具原始产物       : $(Join-Path $OutDir "_per_tool")
每 DLL 明细报告      : $ReportCsv
日志                  : $LogFile
=========================================================
"@
Add-Content -LiteralPath $LogFile -Value $summary -Encoding UTF8
Write-Host ""
Write-Host $summary -ForegroundColor Cyan

if ($allFail -gt 0) {
  Write-Warning "有 $allFail 个 DLL 双路线都失败，请打开 $ReportCsv 查看 FailReason，并把失败的那几行贴回来处理。"
  exit 2
}
exit 0
