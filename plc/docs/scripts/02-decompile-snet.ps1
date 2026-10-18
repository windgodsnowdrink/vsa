<#
.SYNOPSIS
  对 E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet 下每个包的最佳 TFM DLL，
  使用 ilspycmd (ICSharpCode.ILSpyCmd) 批量反编译，每类一个 .cs 文件。
.PREREQ
  - .NET 8.0+ SDK (dotnet) 已安装并可用 dotnet tool 安装
  - 若已在别的机器上跑，需能访问 nuget.org（拉 ilspycmd）
.OUTPUT
  E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\src\{Package}\{Namespace}\{Type}.cs
#>

param(
  [string]$SnetRoot      = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet",
  [string]$InventoryCsv  = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\snet-dll-inventory.csv",
  [string]$OutDir        = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\src",
  [string]$LogFile       = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\decompile.log",
  [string]$ILSpyCmdVer   = "8.3.0.7756"
)

$ErrorActionPreference = "Continue"

# -------- 1) 检查 dotnet / 安装 ilspycmd local tool --------
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $here
try {
  $null = Get-Command dotnet -ErrorAction Stop
} catch {
  throw "[DECOMP] 未找到 dotnet 命令，请安装 .NET SDK (>= 8.0)。"
}

# 创建 manifest（如果不存在）并安装 ilspycmd
if (-not (Test-Path (Join-Path $here ".config\dotnet-tools.json"))) {
  dotnet new tool-manifest | Out-Null
}
Write-Host "[DECOMP] 安装/恢复 ilspycmd $ILSpyCmdVer ..." -ForegroundColor Cyan
dotnet tool install ICSharpCode.ILSpyCmd --version $ILSpyCmdVer 2>&1 | Out-Null

# -------- 2) 加载 inventory（缺则先跑 01） --------
if (-not (Test-Path -LiteralPath $InventoryCsv)) {
  Write-Warning "[DECOMP] 缺少 inventory 文件，先调用 01-inventory-snet.ps1 ..."
  & (Join-Path $here "01-inventory-snet.ps1") -SnetRoot $SnetRoot -OutDir (Split-Path -Parent $OutDir)
}
$inv = Import-Csv -LiteralPath $InventoryCsv
if (-not $inv -or $inv.Count -eq 0) { throw "[DECOMP] inventory 为空，请先检查 01 脚本。" }

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
Clear-Content -LiteralPath $LogFile -ErrorAction SilentlyContinue

function Write-Log([string]$m) {
  $line = (Get-Date -Format 'yyyy-MM-dd HH:mm:ss') + "  " + $m
  Add-Content -LiteralPath $LogFile -Value $line -Encoding UTF8
  Write-Host $line
}

# -------- 3) 逐 DLL 反编译 --------
$total = $inv.Count
$idx   = 0
$ok    = 0
$fail  = 0

foreach ($row in $inv) {
  $idx++
  $pkg = $row.Package
  $dllPath = $row.Path
  $tfm = $row.Tfm
  if (-not $dllPath -or -not (Test-Path -LiteralPath $dllPath)) {
    Write-Log "[$idx/$total] 跳过 $pkg : DLL 不存在 $dllPath"
    $fail++; continue
  }

  $pkgOut = Join-Path $OutDir $pkg
  New-Item -ItemType Directory -Force -Path $pkgOut | Out-Null

  Write-Log "[$idx/$total] $pkg ($tfm) => $pkgOut"

  # ilspycmd 参数:
  #   ilspycmd -o <outdir> --project-list-style SingleFilePerType
  #            --no-dead-stores --remove-vb-style-casts --inline-casts
  #            --remove-pinvokes-for-iunknown --unicode <assembly.dll>
  $args = @(
    "tool", "run", "ilspycmd",
    "--outputdir", $pkgOut,
    "--project-list-style", "SingleFilePerType",
    "--no-dead-stores",
    "--remove-vb-style-casts",
    "--inline-casts",
    "--unicode",
    # 对每个 DLL 解出全部引用 DLL 的解析根目录：同 snet 根（让 ilspy 能解析 extern 类型）
    "--search-path", $SnetRoot,
    $dllPath
  )

  $proc = Start-Process -FilePath dotnet -ArgumentList $args `
    -NoNewWindow -Wait -PassThru `
    -RedirectStandardError (Join-Path $pkgOut "_ilspy-err.log") `
    -RedirectStandardOutput (Join-Path $pkgOut "_ilspy-out.log") `
    -WorkingDirectory $here

  if ($proc.ExitCode -eq 0) {
    $csCount = (Get-ChildItem -LiteralPath $pkgOut -Recurse -Filter *.cs | Measure-Object).Count
    Write-Log "  OK  ($csCount .cs files)"
    $ok++
  }
  else {
    Write-Log "  FAIL exit=$($proc.ExitCode)  (see $pkgOut\_ilspy-err.log)"
    $fail++
  }
}

# -------- 4) 汇总 --------
Write-Log "========================"
Write-Log "完成。OK=$ok   FAIL=$fail   TOTAL=$total"
Write-Log "输出目录: $OutDir"
Write-Log "日志    : $LogFile"
Pop-Location
