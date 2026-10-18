# plc-host 设备协议插件（Tier1 ALC）

本目录承载 **Tier1 设备协议插件**：不可信、按设备协议隔离、可热重载的独立程序集。
每个插件经 `PluginLoadContext`（可收集 ALC）加载，互不污染、可独立升级，并在启动时自注册到宿主 `DeviceCatalog`。

> 信任边界沿用 ADR-115 两层架构；本层为「不可信 / 设备协议」侧。共享契约程序集 `Plc.Plugins.Contracts`
> 承担类型身份统一角色（`Private=false`，不随插件复制，由宿主默认 ALC 解析）。

---

## 1. 契约（位于 `Plc.Plugins.Contracts`）

| 类型 | 职责 |
|------|------|
| `IPlugin` | 插件生命周期：`Id / Name / Version / StartAsync(IPluginContext) / StopAsync(...)` |
| `IDeviceProtocol` | 协议抽象：`ProtocolId`（如 `modbus.tcp`）、`DisplayName`、`Capabilities`、`CreateSession(DeviceConnectionOptions)` |
| `IDeviceSession : IAsyncDisposable` | 一次连接的生命周期：`ReadAsync(ReadRequest)` / `WriteAsync(WriteRequest)` |
| `DeviceFrame / FrameDirection` | 统一设备帧（ADR-119 决策 ③）：原始字节 + 显式元数据，不可变；`Inbound`/`Outbound` |
| `IDeviceCatalog` | 宿主注册中心（宿主实现，单例注入 DI）：`Register / Get / All` |
| `DeviceConnectionOptions / ReadRequest / WriteRequest / RegisterType / DeviceCapabilities` | 值类型；`RegisterType`：`Coil=1, DiscreteInput=2, HoldingRegister=3, InputRegister=4` |

#### 1.1 `IDeviceSession` 可选成员（默认接口方法，ADR-119）

以下成员均有默认实现，**现有插件零改动即可编译**；按需覆写可获得数据面优化：

| 成员 | 默认值 | 覆写收益 |
|------|--------|----------|
| `string ProtocolId` | `""` | 帧元数据带上协议标识，便于链路追踪与门禁断言 |
| `bool SupportsConnectionReuse` | `false` | 置 `true` 表示连接可跨请求复用 → 宿主将其放入会话池，省去每次建连开销 |
| `Task<IReadOnlyList<DeviceFrame>> ReadBatchAsync(requests, deviceId, ct)` | 串行转调 `ReadAsync` | 合并读（如 Modbus 把连续地址合成一次 0x03）是吞吐主要来源；返回须与入参**同序同长**，单项失败落 `DeviceFrame.Failure(...)` 而非抛异常 |

> ⚠️ **DIM 可见性**：默认接口成员只在**接口**上可见。`var s = new MySession(); s.ReadBatchAsync(...)`
> 编译不过——必须按接口编程：`IDeviceSession s = new MySession();`。

> ⚠️ **类型身份陷阱**：插件与宿主必须共享同一个 `Plc.Plugins.Contracts` 程序集实例。
> `PluginLoadContext.Load` 对 `Plc.Plugins.Contracts` 返回 `null` → 回退到宿主默认 ALC 加载，
> 从而避免「插件侧与宿主侧各加载一份契约」导致的 `IDeviceProtocol` 类型不相等、注册中心拒绝注册。

---

## 2. 编写一个新插件

### 2.1 工程骨架 `plugins/<Name>/<Name>.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net11.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <AssemblyName><Name></AssemblyName>
  </PropertyGroup>
  <ItemGroup>
    <!-- Private=false：契约层不随插件复制，由宿主 ALC 以共享程序集方式解析，保证类型身份一致 -->
    <ProjectReference Include="..\..\..\Plc.Plugins.Contracts\Plc.Plugins.Contracts.csproj">
      <Private>false</Private>
    </ProjectReference>
  </ItemGroup>
  <!-- 构建后将插件 DLL 拷贝到宿主可发现的 plugins/<Name>/bin/（PluginManager 用 AllDirectories 扫描） -->
  <Target Name="CopyPluginToHost" AfterTargets="Build">
    <ItemGroup><_PluginDll Include="$(TargetDir)$(AssemblyName).dll" /></ItemGroup>
    <MakeDir Directories="$(MSBuildThisFileDirectory)bin" />
    <Copy SourceFiles="@(_PluginDll)" DestinationFolder="$(MSBuildThisFileDirectory)bin" SkipUnchangedFiles="true" />
  </Target>
</Project>
```

### 2.2 实现（同时实现 `IPlugin` 与 `IDeviceProtocol`）

```csharp
public sealed class DemoDevicePlugin : IPlugin, IDeviceProtocol
{
    public string Id => "demo.device";
    public string Name => "Demo Device Protocol";
    public string Version => "1.0.0";

    public string ProtocolId => "demo.device";          // 网关路由键：/api/devices/demo.device/read
    public string DisplayName => "Demo Device";
    public DeviceCapabilities Capabilities =>
        new(supportsRead: true, supportsWrite: true, supportedTypes: new[] { RegisterType.HoldingRegister, RegisterType.InputRegister, RegisterType.Coil, RegisterType.DiscreteInput });

    public IDeviceSession CreateSession(DeviceConnectionOptions options) => new DemoSession(options);

    public Task StartAsync(IPluginContext ctx, CancellationToken ct)
    {
        // 自注册到宿主目录，使 /api/devices/protocols 与 /api/devices/{protocolId}/read|write 可用
        ctx.Services.GetRequiredService<IDeviceCatalog>().Register(this);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
```

`IDeviceSession` 基于 `System.Net.Sockets`（或协议 SDK）实现 `ReadAsync` / `WriteAsync`，
**不要引入与宿主冲突的传递依赖**（契约层 `Private=false` 已隔离）。

### 2.3 接入宿主构建（一次性，在 `plc-host.csproj`）

每新增一个插件，需在 `plc-host.csproj` 增加两项，使其被构建并复制到运行时：

```xml
<!-- (1) 作为构建依赖触发各自编译，但不进宿主依赖图 / 默认 ALC -->
<ProjectReference Include="plugins\<Name>\<Name>.csproj" ReferenceOutputAssembly="false" Private="false" />

<!-- (2) AfterBuild 目标 CopyPluginAssemblies 已统一把 plugins/<Name>/bin/*.dll 复制到 $(OutDir)plugins/<Name>/bin/ -->
```

> 现有 7 个插件已登记（见 §4）。**不要**用 `<Compile Include>` 把插件源码纳入宿主编译——已用
> `<Compile Remove="plugins\**\*.cs" />` 排除，防止与插件工程同名类型冲突。

---

## 3. 加载、发现与热重载

- `PluginManager.DiscoverAndLoad()`：`plugins/<Name>/<Name>.dll` → `PluginLoadContext` ALC 加载 → 经无参 ctor 实例化 `IPlugin`。
- `PluginManager.StartAllAsync()`：调用每个插件的 `StartAsync`，插件在内部自注册到 `IDeviceCatalog`。
- 宿主启动：`Program.cs` 注册 `PluginManager` 单例 + `PluginHostService`（`IHostedService`），在 `StartAsync` 内 `DiscoverAndLoad()` + `StartAllAsync()`。
- **热重载**：`PluginManager` 对 `plugins/` 挂 `FileSystemWatcher`，DLL 变更即重新加载（可收集 ALC，旧 Assembly 可回收）。
- 路由暴露（`DeviceProtocolsModule : ICarterModule`）：
  - `GET /api/devices/protocols` —— 已注册协议清单（HMI/AIOT 发现可用协议）
  - `GET/POST /api/devices/{protocolId}/read|write` —— 网关式透传；未注册协议返回 `404`
  - `GET /plugins` —— 已加载插件清单（`id/name/version`）
  - `GET /healthz` —— 插件计数

---

## 4. 协议矩阵（当前覆盖）

### 4.1 已落地真实插件（ADR-118 目标矩阵，100% 覆盖）

| ProtocolId | 工程 | 协议 | 读/写 | 备注 |
|------------|------|------|-------|------|
| `modbus.tcp` | DemoDevicePlugin | Modbus TCP | ✅/✅ | MBAP+PDU 真实帧；含 `len-1` PDU 读修复 |
| `melsec.mc` | MelsecDevicePlugin | MELSEC MC Qna-3E | ✅/✅ | 二进制帧编解码，真实 `TcpClient` |
| `fatek` | FatekDevicePlugin | FATEK | ✅/✅ | — |
| `fuji.sph` | FujiDevicePlugin | Fuji MICREX-SPH | ✅/✅ | — |
| `keyence.kv` | KeyenceDevicePlugin | Keyence KV | ✅/✅ | — |
| `lora.wan` | LoRaDevicePlugin | LoRaWAN | ✅/✅ | — |
| `bacnet.ip` | BacnetDevicePlugin | BACnet/IP | ✅/✅ | — |

### 4.2 扩展候选（SNET 参考集，参考-only，未接入运行时）

SNET 生产组件已反编译还原（41+ 驱动，含 Siemens / Omron / Allen-Bradley / Beckhoff / Mitsubishi /
Delta / Inovance / Fanuc / Panasonic / OpcUa / …）。**规范全量语料见 `docs/snet/decompiled/`**
（2026-09-02 已从 `plugins/Snet.*` 去壳：该处曾为与其逐字节重复的轻量副本，删除不影响任何运行行为）。
新增协议时，以 `Snet.XxxOperate` 为参考实现适配器 `XxxDevicePlugin : IDeviceProtocol` 即可。

---

## 5. 反编译参考文件的位置约定

- **不在** `src/plc-host/plugins/`（该命名空间仅容纳可构建的 ALC 插件）。
- **在** `docs/snet/decompiled/<Assembly>/<Assembly>.decompiled.cs`（可查阅参考）。
- 集成蓝图与八项需求映射见 `docs/snet/RECOVERY_AND_INTEGRATION.md`。

---

## 6. 本地验证

```bash
# 构建宿主（会触发 7 插件编译 + 复制 DLL 到运行时 plugins/）
dotnet build src/plc-host -c Debug

# 端到端（自动选端口，含 Tier1 插件体系 5 项检查）
bash scripts/verify-cockpit.sh
# 关注：GET /plugins 列 7 插件；GET /api/devices/protocols 列 7 协议；
#       POST /api/devices/modbus.tcp/read 指向不可达主机返回非 404（证明路由到已注册插件）
```
