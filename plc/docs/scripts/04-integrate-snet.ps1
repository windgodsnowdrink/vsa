<#
.SYNOPSIS
  把 snet 反编译出的源码整合到 plc 仓库：
    1) 按包 + 命名空间目录拷贝到 /src/snet/{Package}/
    2) 为每个包生成 SDK 风格 csproj (TargetFramework=net11.0)
    3) 生成 Snet.Protocols 整体打包的目录入口 + Directory.Build.props
    4) 扫描反编译出的协议类（S7/Modbus/Melsec/Omron/Fins/BACnet/LoRa 等），
       为每个类生成一个适配器 partial 类，实现 plc-vsa-demo 中 IDeviceProtocol
.PREREQ
  已运行 01 + 02 (或 03)，反编译输出在:
    E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\src\
.INPUT/OUTPUT
  全部写入 $RepoRoot\src\snet\ （不会污染其他项目）
#>

param(
  [string]$DecRoot   = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\src",
  [string]$RepoRoot  = "E:\WorkSpace\windgodsnowdrink\vsa\plc",
  [string]$Inventory = "E:\WorkSpace\windgodsnowdrink\vsa\plc\docs\snet-decompiled\snet-dll-inventory.csv"
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $DecRoot)) {
  throw "[INTEG] 反编译输出目录不存在: $DecRoot，请先运行 01 + 02/03。"
}

$DstRoot = Join-Path $RepoRoot "src\snet"
New-Item -ItemType Directory -Force -Path $DstRoot | Out-Null

# ------- 1) Directory.Build.props：整包共享 -------
$props = @"
<Project>
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>Snet.\$(MSBuildProjectName.Replace('Snet.',''))</RootNamespace>
    <AssemblyName>Snet.\$(MSBuildProjectName.Replace('Snet.',''))</AssemblyName>
    <GenerateAssemblyInfo>true</GenerateAssemblyInfo>
    <!-- 反编译代码有大量命名/注释/过时API，关闭非致命分析 -->
    <EnableNETAnalyzers>false</EnableNETAnalyzers>
    <AnalysisLevel>none</AnalysisLevel>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <NoWarn>$(NoWarn);CS1570;CS1573;CS1574;CS1580;CS1584;CS1591;CS1710;CS1711;CS1734;CS1735;CS0108;CS0109;CS0114;CS0169;CS0219;CS0414;CS0436;CS0618;CS0612;CS0672;CS0809;CS1572</NoWarn>
  </PropertyGroup>
</Project>
"@
Set-Content -LiteralPath (Join-Path $DstRoot "Directory.Build.props") -Value $props -Encoding UTF8

# ------- 2) 每包拷贝 + csproj 生成 -------
$inv = @{}
if (Test-Path -LiteralPath $Inventory) {
  Import-Csv -LiteralPath $Inventory | ForEach-Object { $inv[$_.Package] = $_ }
}

$packages = Get-ChildItem -LiteralPath $DecRoot -Directory | Sort-Object Name
$projFiles = @()
$protocolCandidates = New-Object System.Collections.Generic.List[object]

foreach ($pkg in $packages) {
  $pkgName  = $pkg.Name
  $dstPkg   = Join-Path $DstRoot $pkgName
  if (Test-Path -LiteralPath $dstPkg) { Remove-Item -LiteralPath $dstPkg -Recurse -Force }
  Copy-Item -LiteralPath $pkg.FullName -Destination $dstPkg -Recurse

  # 去掉反编译过程的日志/驱动文件
  Get-ChildItem -LiteralPath $dstPkg -Recurse -File -Include "_*.log","_*.csx","_*.error.txt","_ilspy-*.log","_dnspy-*.log","_driver*" |
    Remove-Item -Force -ErrorAction SilentlyContinue

  # 分析该 DLL 的引用依赖（来自 inventory）
  $pkgRefs = @(
    if (Test-Path -LiteralPath (Join-Path (Split-Path -Parent $Inventory) "snet-assembly-refs.csv")) {
      Import-Csv (Join-Path (Split-Path -Parent $Inventory) "snet-assembly-refs.csv") |
        Where-Object { $_.Package -eq $pkgName } | Select-Object -ExpandProperty RefName -Unique
    } else { @() }
  )
  # snet 内部包之间的引用 → ProjectReference
  $internalRefs = $packages.Name | Where-Object { $_ -in $pkgRefs -or "$_.dll" -in $pkgRefs }

  $csprojXml = [System.Text.StringBuilder]::new()
  [void]$csprojXml.AppendLine("<Project Sdk=`"Microsoft.NET.Sdk`">")
  [void]$csprojXml.AppendLine("  <PropertyGroup>")
  [void]$csprojXml.AppendLine("    <AssemblyName>$pkgName</AssemblyName>")
  [void]$csprojXml.AppendLine("    <RootNamespace>$pkgName</RootNamespace>")
  if ($inv[$pkgName] -and $inv[$pkgName].AsmVersion) {
    [void]$csprojXml.AppendLine("    <Version>$($inv[$pkgName].AsmVersion)</Version>")
  }
  [void]$csprojXml.AppendLine("  </PropertyGroup>")
  [void]$csprojXml.AppendLine("  <ItemGroup>")
  foreach ($r in $internalRefs) {
    [void]$csprojXml.AppendLine("    <ProjectReference Include=`"../$r/$r.csproj`" />")
  }
  [void]$csprojXml.AppendLine("  </ItemGroup>")
  # 常用外部依赖（按名称猜测），如果没命中就加注释
  [void]$csprojXml.AppendLine("  <!-- 自动猜测的外部依赖（按需取消注释/改版本） -->")
  [void]$csprojXml.AppendLine("  <ItemGroup Condition=`"'false'=='true'`">")
  $guessMap = @{
    "System.IO.Ports"="8.0.0";
    "System.Net.Sockets"="默认框架";
    "Microsoft.Extensions.Logging.Abstractions"="9.0.0";
    "Microsoft.Extensions.DependencyInjection.Abstractions"="9.0.0";
    "Newtonsoft.Json"="13.0.3";
    "System.Text.Json"="默认框架";
    "System.Memory"="默认框架";
    "System.Buffers"="默认框架";
    "NLog"="5.3.5";
    "Microsoft.VisualStudio.Threading"="18.7.23";
    "StreamJsonRpc"="2.21.69";
    "Disruptor"="6.0.1";
    "McMaster.NETCore.Plugins"="1.0.0";
  }
  foreach ($refName in $pkgRefs) {
    if ($guessMap.ContainsKey($refName) -and $guessMap[$refName] -ne "默认框架") {
      [void]$csprojXml.AppendLine("    <PackageReference Include=`"$refName`" Version=`"$($guessMap[$refName])`" />")
    }
  }
  [void]$csprojXml.AppendLine("  </ItemGroup>")
  [void]$csprojXml.AppendLine("</Project>")
  $projPath = Join-Path $dstPkg "$pkgName.csproj"
  Set-Content -LiteralPath $projPath -Value $csprojXml.ToString() -Encoding UTF8
  $projFiles += $projPath

  # ------- 3) 扫描本包中"看起来是协议类"的类型名 -------
  $protoKeywords = @("S7Client","Siemens","Modbus","ModbusTcp","ModbusRtu","Melsec","McProtocol","Omron","Fins",
                     "FinsTcp","FinsUdp","Fuji","Keyence","Kv","Bacnet","LoRa","Fatek","AllenBradley",
                     "CIP","EtherNetIP","S5","Delta","Panasonic","Mitsubishi","SiemensS7","DeviceNet")
  $csFiles = Get-ChildItem -LiteralPath $dstPkg -Recurse -Filter *.cs |
    Where-Object { $_.Name -notlike "_*" }
  foreach ($cs in $csFiles) {
    $nameNoExt = [IO.Path]::GetFileNameWithoutExtension($cs.Name)
    foreach ($kw in $protoKeywords) {
      if ($nameNoExt -match [regex]::Escape($kw) -or $cs.Directory.Name -match [regex]::Escape($kw)) {
        # 粗读文件识别命名空间 / 类型定义
        $first200 = Get-Content -LiteralPath $cs.FullName -TotalCount 200 -Raw
        if ($first200 -match '(?m)^\s*namespace\s+([\w\.]+)') { $ns = $matches[1] } else { $ns = "" }
        if ($first200 -match '(?m)^\s*(?:public\s+)?(?:sealed\s+)?(?:unsafe\s+)?(?:partial\s+)?class\s+(\w+)') { $cls = $matches[1] } else { $cls = $nameNoExt }
        $protocolCandidates.Add([pscustomobject]@{
          Package  = $pkgName
          File     = $cs.FullName
          Namespace= $ns
          Class    = $cls
          Keyword  = $kw
        })
        break
      }
    }
  }
}

# ------- 4) 候选协议类清单（供我写适配器） -------
$protoCsv = Join-Path $DstRoot "protocol-candidates.csv"
$protoTxt = Join-Path $DstRoot "protocol-candidates.txt"
$protocolCandidates | Export-Csv -LiteralPath $protoCsv -NoTypeInformation -Encoding UTF8

$sb = New-Object System.Text.StringBuilder
[void]$sb.AppendLine("=== snet 协议类候选清单（共 $($protocolCandidates.Count) 项）===")
[void]$sb.AppendLine("来源: $DecRoot")
[void]$sb.AppendLine("")
foreach ($p in ($protocolCandidates | Sort-Object Package,Keyword,Class)) {
  [void]$sb.AppendLine(("[{0,-30}] kw={1,-12} {2,-30} class {3,-30}  @ {4}" -f
    $p.Package, $p.Keyword, $p.Namespace, $p.Class,
    $p.File.Substring($DstRoot.Length+1)))
}
Set-Content -LiteralPath $protoTxt -Value $sb.ToString() -Encoding UTF8

# ------- 5) 生成每个候选协议类的适配器占位 -------
$adaptersDir = Join-Path $RepoRoot "src\snet\Snet.Adapters"
New-Item -ItemType Directory -Force -Path $adaptersDir | Out-Null

$sbCtorCalls = New-Object System.Text.StringBuilder
foreach ($p in ($protocolCandidates | Sort-Object Package,Class | Get-Unique -AsString -OnProperty { $_.Package + $_.Class })) {
  $safeNs  = if ([string]::IsNullOrEmpty($p.Namespace)) { "Snet.$($p.Package)" } else { $p.Namespace }
  $adapter = @"
// <auto-generated by 04-integrate-snet.ps1 />
//
// 手动完善建议：
//   1) 打开 $($p.File.Substring($RepoRoot.Length+1))
//   2) 在 $($safeNs).$($p.Class) 上找到公开的 Connect/Disconnect/Read/Write 方法
//   3) 在下面的桥接方法中按实际签名填充（现在只是最小可编译壳）
using global::System.Threading;
using global::System.Threading.Tasks;
using global::PlcAiot.Abstractions.Devices;  // IDeviceProtocol / RegisterType
using global::PlcAiot.Models;                 // Result<T>
using Orig = $safeNs.$($p.Class);
namespace Snet.Adapters;

/// <summary>桥接 snet 原类 <c>$($p.Class)</c> → VSA 的 <see cref="IDeviceProtocol"/>。</summary>
public sealed class $($p.Class)_Adapter : IDeviceProtocol
{
    private readonly Orig _impl = new();

    // ---- IDeviceProtocol 基本契约 ----
    public string DeviceType => "$($p.Keyword)";
    public bool   Connected  => _internal_Connected();  // TODO: 按原类真实属性替换

    public ValueTask ConnectAsync(string host, int port, CancellationToken ct = default)
    {
        // TODO: 替换为原类实际的 Connect / Open 方法（可能是同步或异步）
        // e.g. _impl.Connect(host, port);
        return default;
    }

    public ValueTask DisconnectAsync(CancellationToken ct = default)
    {
        // TODO: 替换为原类实际的 Disconnect / Close / Dispose
        (_impl as IDisposable)?.Dispose();
        return default;
    }

    public async ValueTask<Result<T?>> Read<T>(string address, RegisterType type, CancellationToken ct = default)
    {
        // TODO: 映射到原类读取方法：_impl.Read(address, count) 等
        await Task.Yield();
        return Result.FromValue(default(T?));
    }

    public async ValueTask<Result> Write<T>(string address, T value, RegisterType type, CancellationToken ct = default)
    {
        // TODO: 映射到原类写入方法
        await Task.Yield();
        return Result.Ok();
    }

    public async ValueTask<Result<T[]>> ReadBatch<T>(string startAddress, int count, RegisterType type, CancellationToken ct = default)
    {
        await Task.Yield();
        return Result.FromValue(System.Array.Empty<T>());
    }

    public async ValueTask<Result> WriteBatch<T>(string startAddress, T[] values, RegisterType type, CancellationToken ct = default)
    {
        await Task.Yield();
        return Result.Ok();
    }

    public void Dispose() => (_impl as IDisposable)?.Dispose();

    // ---- 占位帮助器：按需替换为原类真实状态 ----
    private bool _internal_Connected()
    {
        // 例如 return _impl.IsConnected;
        return false;
    }
}

"@
  $outFile = Join-Path $adaptersDir ("$($p.Package)-$($p.Class).Adapter.cs")
  Set-Content -LiteralPath $outFile -Value $adapter -Encoding UTF8
  [void]$sbCtorCalls.AppendLine("            // builder.Services.AddSingleton<IDeviceProtocol, Snet.Adapters.$($p.Class)_Adapter>();")
}

$adaptersProj = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <LangVersion>preview</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <AssemblyName>Snet.Adapters</AssemblyName>
    <RootNamespace>Snet.Adapters</RootNamespace>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
    <NoWarn>`$(NoWarn);CS0169;CS0219;CS0414;CS0618</NoWarn>
  </PropertyGroup>
  <ItemGroup>
    <!-- 对每个候选协议所属的 snet 包生成 ProjectReference（自动） -->
$($packages.Name | ForEach-Object { "    <ProjectReference Include=`"../$_/$_.csproj`" />" } | Out-String)
    <ProjectReference Include="../../PlcAiot.Abstractions/PlcAiot.Abstractions.csproj" />
  </ItemGroup>
</Project>
"@
Set-Content -LiteralPath (Join-Path $adaptersDir "Snet.Adapters.csproj") -Value $adaptersProj -Encoding UTF8

# ------- 6) 最终写入集成入口 README-like 代码片段 -------
$wireup = @"
/* ===========================================================
 * 在 plc-vsa-demo.cs / plc-saas 的 Program / CompositionRoot 中，
 * 把 snet 适配器接入 DI：
 *   1) builder.Services.AddSingleton<IDeviceProtocol, Snet.Adapters.S7Client_Adapter>();
 *   2) builder.Services.AddSingleton<IDeviceProtocol, Snet.Adapters.ModbusTcp_Adapter>();
 *   3) ... 下面是自动生成的候选 (每行一条，按需取消注释)
 * =========================================================== */
// 来源: snet/src/protocol-candidates.csv  共 $($protocolCandidates.Count) 个候选
$($sbCtorCalls.ToString())
"@
Set-Content -LiteralPath (Join-Path $DstRoot "WireUpSnippet.cs.txt") -Value $wireup -Encoding UTF8

Write-Host "[INTEG] 完成" -ForegroundColor Green
Write-Host "  项目目录   : $DstRoot   ($($projFiles.Count) packages + Snet.Adapters)"
Write-Host "  候选协议类 : $($protocolCandidates.Count)  — 清单 $protoTxt"
Write-Host "  适配器源码 : $adaptersDir   (每个候选一个 Adapter 壳，按需填)"
