<#
.SYNOPSIS
  盘点 E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet 下的所有 DLL（按 NuGet 包目录结构）。
  输出:
    snet-dll-inventory.txt       人类可读清单
    snet-dll-inventory.csv       可导入 Excel
    snet-assembly-refs.csv       每个 DLL 的引用程序集列表
#>

param(
  [string]$SnetRoot = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet",
  [string]$OutDir   = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $SnetRoot)) {
  throw "[SNET-INV] 目录不存在: $SnetRoot"
}

New-Item -ItemType Directory -Force -Path $OutDir | Out-Null

# --- 1) 枚举所有 DLL，按 package\tfm 分组 ---
$dlls = Get-ChildItem -LiteralPath $SnetRoot -Recurse -File -Filter *.dll |
  Where-Object { $_.FullName -match '[\\/]lib[\\/](?<tfm>[^\\/]+)[\\/][^\\/]+\.dll$' -or
                 $_.Directory.Name -eq "lib" -or
                 -not ($_.Name -like "*.resources.dll") } |
  Sort-Object FullName

Write-Host "[SNET-INV] 扫描到 DLL: $($dlls.Count) 个" -ForegroundColor Cyan

# TFM 优先级（越大越优先），用户 net10.0 是最新
$tfmRank = @{
  "net11.0"       = 110
  "net10.0"       = 100
  "net9.0"        = 90
  "net8.0"        = 80
  "net7.0"        = 70
  "net6.0"        = 60
  "netstandard2.1"= 21
  "netstandard2.0"= 20
  "net481"        = 481
  "net48"         = 480
  "net472"        = 472
}

function Get-TfmRank([string]$t) {
  if ($tfmRank.ContainsKey($t)) { return $tfmRank[$t] }
  if ($t -match '^net(\d+)\.(\d+)(-.*)?$') { return [int]$matches[1]*10 + [int]$matches[2] }  # e.g. net11.0-windows
  return 0
}

# 按包名聚合（包名 = snet 下一级目录名）
$pkgGroups = @{}
foreach ($d in $dlls) {
  $rel = $d.FullName.Substring($SnetRoot.Length).TrimStart('\','/')
  $parts = $rel -split '[\\/]'                   # e.g. snet.core, lib, net10.0, Snet.Core.dll
  $pkg = if ($parts.Count -ge 1) { $parts[0] } else { "_root_" }
  $tfm = if ($parts.Count -ge 3 -and $parts[1] -ieq 'lib') { $parts[2] } else { "unknown" }
  if (-not $pkgGroups.ContainsKey($pkg)) { $pkgGroups[$pkg] = @() }
  $pkgGroups[$pkg] += [pscustomobject]@{
    Package = $pkg
    Tfm     = $tfm
    DllName = $d.Name
    Path    = $d.FullName
    SizeKB  = [math]::Round($d.Length / 1KB, 1)
    TfmRank = (Get-TfmRank $tfm)
  }
}

# 每个包挑优先级最高的 TFM 的 DLL 作为"主反编译目标"
$best = @()
foreach ($k in $pkgGroups.Keys) {
  $grp = $pkgGroups[$k] | Sort-Object TfmRank -Descending
  $best += $grp | Where-Object { $_.TfmRank -eq $grp[0].TfmRank }
}

# --- 2) 读程序集元数据（引用、版本、公钥token） ---
$refRows = @()
$asmRows = foreach ($b in $best) {
  try {
    $an = [System.Reflection.AssemblyName]::GetAssemblyName($b.Path)
    # Reflect 引用列表（不加载到执行上下文）
    $ads = [System.Reflection.Assembly]::ReflectionOnlyLoadFrom($b.Path)
    $refs = $ads.GetReferencedAssemblies()
    foreach ($r in $refs) {
      $refRows += [pscustomobject]@{
        Package     = $b.Package
        Dll         = $b.DllName
        Tfm         = $b.Tfm
        RefName     = $r.Name
        RefVersion  = $r.Version.ToString()
        RefPublicKeyToken = ($r.GetPublicKeyToken() | ForEach-Object { $_.ToString('x2') }) -join ''
      }
    }
    [pscustomobject]@{
      Package          = $b.Package
      Tfm              = $b.Tfm
      Dll              = $b.DllName
      AsmName          = $an.Name
      AsmVersion       = $an.Version.ToString()
      PublicKeyToken   = ($an.GetPublicKeyToken() | ForEach-Object { $_.ToString('x2') }) -join ''
      SizeKB           = $b.SizeKB
      RefCount         = $refs.Count
      Path             = $b.Path
    }
  }
  catch {
    [pscustomobject]@{
      Package = $b.Package; Tfm = $b.Tfm; Dll = $b.DllName
      AsmName = ""; AsmVersion = ""; PublicKeyToken = ""; SizeKB = $b.SizeKB
      RefCount = -1; Path = $b.Path
    }
    Write-Warning "[SNET-INV] 读取失败 $($b.Path): $($_.Exception.Message)"
  }
}

# --- 3) 写文件 ---
$txtPath  = Join-Path $OutDir "snet-dll-inventory.txt"
$csvPath  = Join-Path $OutDir "snet-dll-inventory.csv"
$refPath  = Join-Path $OutDir "snet-assembly-refs.csv"

$asmRows | Sort-Object Package | Export-Csv -NoTypeInformation -Encoding UTF8 -Path $csvPath
$refRows | Sort-Object Package,Dll,RefName | Export-Csv -NoTypeInformation -Encoding UTF8 -Path $refPath

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("=== snet DLL inventory ===")
[void]$sb.AppendLine("Root   : $SnetRoot")
[void]$sb.AppendLine("Packages: $($pkgGroups.Count)   Selected DLLs: $($asmRows.Count)   All DLLs: $($dlls.Count)")
[void]$sb.AppendLine("")
foreach ($a in ($asmRows | Sort-Object Package)) {
  [void]$sb.AppendLine(("{0,-30} {1,-16} {2,8}KB  refs={3,-3}  {4}" -f
    $a.Package, $a.Tfm, $a.SizeKB, $a.RefCount, $a.Dll))
}
[void]$sb.AppendLine("")
[void]$sb.AppendLine("CSV : $csvPath")
[void]$sb.AppendLine("REF : $refPath")
Set-Content -Path $txtPath -Value $sb.ToString() -Encoding UTF8

Write-Host "[SNET-INV] 完成" -ForegroundColor Green
Write-Host "  TXT: $txtPath"
Write-Host "  CSV: $csvPath"
Write-Host "  REF: $refPath"
