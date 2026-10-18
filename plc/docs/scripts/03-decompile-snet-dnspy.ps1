<#
.SYNOPSIS
  使用用户本机 dnSpy 目录下的 dnSpy.Console / ILSpy 核心（ICSharpCode.Decompiler）
  批量反编译 snet 下的每个 DLL。
.PREREQ
  dnSpy 目录: E:\Net Tool\dnSpy\
  优先使用:
    1) dnSpy.Console.exe 或 dnSpy.Console.dll（社区版 dnSpyEx 通常自带 headless 控制台）
    2) 否则退回到反射 + IL 级别导出 + 用 ICSharpCode.Decompiler.dll 手工创建 decompiler
.INPUT
  依赖 01-inventory-snet.ps1 生成的 snet-dll-inventory.csv
.OUTPUT
  E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\src\{Package}\{Namespace}\{Type}.cs
#>

param(
  [string]$SnetRoot      = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet",
  [string]$DnSpyDir      = "E:\Net Tool\dnSpy",
  [string]$InventoryCsv  = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\snet-dll-inventory.csv",
  [string]$OutDir        = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\src",
  [string]$LogFile       = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\decompile-dnspy.log"
)

$ErrorActionPreference = "Continue"

if (-not (Test-Path -LiteralPath $DnSpyDir)) {
  throw "[DNSPY] 目录不存在: $DnSpyDir"
}

# ---------- 1) 识别可用的 dnSpy headless 工具 ----------
$dnConsoleExe = Join-Path $DnSpyDir "dnSpy.Console.exe"
$dnConsoleDll = Join-Path $DnSpyDir "dnSpy.Console.dll"
$decDll       = Join-Path $DnSpyDir "ICSharpCode.Decompiler.dll"

$mode = $null
if (Test-Path -LiteralPath $dnConsoleExe) {
  $mode = "dnSpy.Console.exe"
}
elseif (Test-Path -LiteralPath $dnConsoleDll) {
  $mode = "dotnet dnSpy.Console.dll"
}
elseif (Test-Path -LiteralPath $decDll) {
  $mode = "ICSharpCode.Decompiler"
}
else {
  # 兜底：提示用户先用 GUI 或找其他可执行
  throw "[DNSPY] 在 $DnSpyDir 下找不到 dnSpy.Console(.exe/.dll) 或 ICSharpCode.Decompiler.dll；可选替代：GUI 批量拖入后导出，或改用 02-decompile-snet.ps1（ilspycmd 路线）。"
}

Write-Host "[DNSPY] 使用模式: $mode" -ForegroundColor Cyan

# ---------- 2) 加载 inventory（缺则先跑 01） ----------
$scriptsDir = Split-Path -Parent $MyInvocation.MyCommand.Path
if (-not (Test-Path -LiteralPath $InventoryCsv)) {
  Write-Warning "[DNSPY] 缺少 inventory 文件，先调用 01-inventory-snet.ps1 ..."
  & (Join-Path $scriptsDir "01-inventory-snet.ps1") -SnetRoot $SnetRoot -OutDir (Split-Path -Parent $OutDir)
}
$inv = Import-Csv -LiteralPath $InventoryCsv
if (-not $inv -or $inv.Count -eq 0) { throw "[DNSPY] inventory 为空。" }

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
Clear-Content -LiteralPath $LogFile -ErrorAction SilentlyContinue

function Write-Log([string]$m) {
  $line = (Get-Date -Format 'yyyy-MM-dd HH:mm:ss') + "  " + $m
  Add-Content -LiteralPath $LogFile -Value $line -Encoding UTF8
  Write-Host $line
}

# ---------- 3) 逐 DLL 反编译 ----------
$idx = 0; $total = $inv.Count; $ok = 0; $fail = 0

foreach ($row in $inv) {
  $idx++
  $pkg = $row.Package
  $dllPath = $row.Path
  $tfm = $row.Tfm
  if (-not $dllPath -or -not (Test-Path -LiteralPath $dllPath)) {
    Write-Log "[$idx/$total] 跳过 $pkg : DLL 不存在 $dllPath"; $fail++; continue
  }

  $pkgOut = Join-Path $OutDir $pkg
  New-Item -ItemType Directory -Force -Path $pkgOut | Out-Null

  Write-Log "[$idx/$total] $pkg ($tfm) => $pkgOut"

  try {
    switch ($mode) {
      "dnSpy.Console.exe" {
        # dnSpy.Console.exe <assembly> -o <outdir> --single-types
        & $dnConsoleExe $dllPath "-o" $pkgOut "--single-types" `
          "--resolve-path" $SnetRoot 2>&1 | Out-File (Join-Path $pkgOut "_dnspy-out.log") -Encoding UTF8
        if ($LASTEXITCODE -ne 0) { throw "dnSpy.Console.exe exit=$LASTEXITCODE" }
      }
      "dotnet dnSpy.Console.dll" {
        & dotnet $dnConsoleDll $dllPath "-o" $pkgOut "--single-types" `
          "--resolve-path" $SnetRoot 2>&1 | Out-File (Join-Path $pkgOut "_dnspy-out.log") -Encoding UTF8
        if ($LASTEXITCODE -ne 0) { throw "dnSpy.Console.dll exit=$LASTEXITCODE" }
      }
      "ICSharpCode.Decompiler" {
        # 用一个小 C# 驱动：加载 ICSharpCode.Decompiler.dll 并逐类型导出
        $driverCS = Join-Path $pkgOut "_driver.csx"
        $code = @"
#r "$decDll"
using System;
using System.IO;
using System.Linq;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
string dll = Environment.GetEnvironmentVariable("DNSPY_DLL")!;
string out = Environment.GetEnvironmentVariable("DNSPY_OUT")!;
string root = Environment.GetEnvironmentVariable("DNSPY_ROOT")!;
Directory.CreateDirectory(out);
var resolver = new DotNetCoreAssemblyResolver(new UniversalAssemblyResolver(dll, false, null), null);
var settings = new DecompilerSettings(LanguageVersion.CSharp13) {
    ThrowOnAssemblyResolveErrors = false,
    UseSdkStyleProjectFormat = true,
};
var dec = new CSharpDecompiler(dll, resolver, settings);
dec.AnalyzerAssemblyResolver = resolver;
foreach (var tp in dec.TypeSystem.MainModule.TypeDefinitions) {
    var ns = string.IsNullOrEmpty(tp.Namespace) ? "_global_" : tp.Namespace;
    var dir = Path.Combine(out, ns.Replace('.', Path.DirectorySeparatorChar));
    Directory.CreateDirectory(dir);
    try {
        var code = dec.DecompileTypeAsString(tp.MetadataToken);
        File.WriteAllText(Path.Combine(dir, tp.Name + ".cs"), code);
    } catch (Exception ex) {
        File.WriteAllText(Path.Combine(dir, tp.Name + ".error.txt"), ex.ToString());
    }
}
Console.WriteLine("done");
"@
        Set-Content -LiteralPath $driverCS -Value $code -Encoding UTF8
        $env:DNSPY_DLL  = $dllPath
        $env:DNSPY_OUT  = $pkgOut
        $env:DNSPY_ROOT = $SnetRoot
        & dotnet script $driverCS 2>&1 | Out-File (Join-Path $pkgOut "_driver.log") -Encoding UTF8
        if ($LASTEXITCODE -ne 0) { throw "driver exit=$LASTEXITCODE" }
      }
    }

    $csCount = (Get-ChildItem -LiteralPath $pkgOut -Recurse -Filter *.cs | Measure-Object).Count
    Write-Log "  OK  ($csCount .cs files)"
    $ok++
  }
  catch {
    Write-Log "  FAIL $_  (see $pkgOut\ _*.log)"
    $fail++
  }
}

Write-Log "========================"
Write-Log "完成。OK=$ok   FAIL=$fail   TOTAL=$total"
Write-Log "输出目录: $OutDir"
Write-Log "日志    : $LogFile"
