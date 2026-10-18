<#
.SYNOPSIS
  反编译 snet：先 dnSpy 路线，失败每个 DLL 自动回退 ilspycmd
.PREREQ
  - .NET SDK 可用 (dotnet)
  - dnSpy 目录: E:\Net Tool\dnSpy
#>

param(
  [string]$SnetRoot     = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet",
  [string]$DnSpyDir     = "E:\Net Tool\dnSpy",
  [string]$OutDir       = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled",
  [string]$ILSpyCmdVer  = "8.3.0.7756"
)

$ErrorActionPreference = "Continue"

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
  $line = (Get-Date -Format "yyyy-MM-dd HH:mm:ss") + "  [$Level]  $m"
  Add-Content -LiteralPath $LogFile -Value $line -Encoding UTF8
  switch ($Level) {
    "OK"   { Write-Host $line -ForegroundColor Green }
    "WARN" { Write-Host $line -ForegroundColor Yellow }
    "ERR"  { Write-Host $line -ForegroundColor Red }
    default { Write-Host $line }
  }
}

# ---------- 1) 盘点 ----------
Write-Log "Step 1: 盘点 snet DLL 清单 ..."
$scriptsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$invScript  = Join-Path $scriptsDir "01-inventory-snet.ps1"
$invCsv     = Join-Path $OutDir "inventory\snet-dll-inventory.csv"
if (-not (Test-Path -LiteralPath $invScript)) { throw "[FATAL] 缺少 01-inventory-snet.ps1" }
& $invScript -SnetRoot $SnetRoot -OutDir (Join-Path $OutDir "inventory")
$inv = @()
if (Test-Path -LiteralPath $invCsv) { $inv = Import-Csv -LiteralPath $invCsv }
if (-not $inv -or $inv.Count -eq 0) { throw "[FATAL] inventory 为空" }
Write-Log "Step 1 完成：$($inv.Count) DLLs"

# ---------- 2) 工具链探测 ----------
Write-Log "Step 2: 准备工具链 ..."
$Mode_dnSpy   = $null
$dnConsoleExe = Join-Path $DnSpyDir "dnSpy.Console.exe"
$dnConsoleDll = Join-Path $DnSpyDir "dnSpy.Console.dll"
$decDll       = Join-Path $DnSpyDir "ICSharpCode.Decompiler.dll"
if      (Test-Path -LiteralPath $dnConsoleExe) { $Mode_dnSpy = "Exe" }
elseif  (Test-Path -LiteralPath $dnConsoleDll) { $Mode_dnSpy = "Dll" }
elseif  (Test-Path -LiteralPath $decDll)       { $Mode_dnSpy = "API" }
else {
  Write-Log "dnSpy 目录无效 -> 全部走 ilspycmd" "WARN"
  $Mode_dnSpy = $null
}

$ilspyOk = $false
try {
  $null = Get-Command dotnet -ErrorAction Stop
  Push-Location $scriptsDir
  if (-not (Test-Path (Join-Path $scriptsDir ".config\dotnet-tools.json"))) {
    dotnet new tool-manifest | Out-Null
  }
  dotnet tool install ICSharpCode.ILSpyCmd --version $ILSpyCmdVer 2>&1 | Out-Null
  if ($LASTEXITCODE -ne 0) {
    dotnet tool uninstall ICSharpCode.ILSpyCmd 2>&1 | Out-Null
    dotnet tool install   ICSharpCode.ILSpyCmd --version $ILSpyCmdVer 2>&1 | Out-Null
  }
  $vTest = (dotnet tool run ilspycmd -- --version 2>&1) -join " "
  Pop-Location
  if ($LASTEXITCODE -eq 0 -or $vTest -match "^\d+\.\d+") { $ilspyOk = $true; Write-Log "ilspycmd 可用: $vTest" }
  else { Write-Log "ilspycmd 验证失败: $vTest" "WARN" }
} catch {
  Write-Log "dotnet/ilspycmd 不可用：$($_.Exception.Message)" "WARN"
  Pop-Location
}

if (-not $Mode_dnSpy -and -not $ilspyOk) { throw "[FATAL] 两条路线都不可用" }

# ---------- 3) 反编译函数 ----------
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
    var dll  = args[0]; var oDir = args[1]; var root = args[2];
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
      name = name.Replace('`','_').Replace('<','_').Replace('>','_').Replace('\\','_').Replace('/','_').Replace('|','_').Replace(':','_').Replace('?','_').Replace('*','_').Replace('"','_');
      try {
        var src = dec.DecompileTypeAsString(t.MetadataToken);
        File.WriteAllText(Path.Combine(dir, name + ".cs"), src); count++;
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

# ---------- 4) 逐 DLL 执行 ----------
Write-Log "Step 3: 逐 DLL 反编译 ..."
$reports = New-Object System.Collections.Generic.List[object]
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
  } else {
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
    } else { $reason = "dnSpy:skipped-unavailable" }

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

  $unified = Join-Path $OutDir "src\$pkg"
  if ($chosen) {
    if (Test-Path -LiteralPath $unified) { Remove-Item -LiteralPath $unified -Recurse -Force }
    Copy-Item -LiteralPath $chosen -Destination $unified -Recurse
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

# ---------- 5) 汇总 ----------
$reports | Export-Csv -NoTypeInformation -Encoding UTF8 -LiteralPath $ReportCsv
$summary = @"
==================== snet 反编译汇总 ====================
Total DLLs           : $total
  dnSpy 路线成功      : $dnSpyWin
  ilspycmd 回退成功   : $ilspyWin
  双路线全部失败      : $allFail
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

Write-Host ""
Write-Host "=========== 关键文件清单，请把以下内容贴回给我 ===========" -ForegroundColor Cyan
Write-Host "--- decompile-report.csv ---"
Import-Csv $ReportCsv | Format-Table Idx,Package,Tfm,Status,Tool,CsFiles -AutoSize | Out-String | Write-Host
Write-Host "--- FAIL / 异常条目 ---"
Import-Csv $ReportCsv | Where-Object { $_.Status -ne "OK" } | Format-List Idx,Package,Tfm,Tool,FailReason | Out-String | Write-Host
Write-Host "--- inventory.txt 前 40 行 ---"
Get-Content (Join-Path $OutDir "inventory\snet-dll-inventory.txt") -TotalCount 40 | Write-Host

if ($allFail -gt 0) { exit 2 }
exit 0
