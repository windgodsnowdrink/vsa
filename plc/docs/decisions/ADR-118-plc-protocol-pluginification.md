# ADR-118 优先级3：PLC 协议插件化（Tier1 ALC 设备协议插件）

- **状态（Status）**：Accepted
- **日期（Date）**：2026-08-22
- **关联**：ADR-114（插件优先战略）、ADR-115（两层模块化架构）、ADR-117（优先级2 能力栈落地）
- **范围**：优先级 3（PLC 插件化）—— 在优先级1 插件宿主 + 优先级2 基础能力栈之上，将 PLC 设备协议以 Tier1 ALC 插件形态落地。

## 0. 背景与目标

优先级 1 已建立独立插件宿主 `plc-host`（DotNetCorePlugins 式 `AssemblyLoadContext` 隔离加载，ADR-114/115）。
优先级 2 已落地基础能力栈（Scrutor 扫描 + Polly 弹性 + Carter 模块化 + HybridCache/ScottPlot/OTel/CommunityToolkit/Foundatio/Dapr，ADR-117）。
优先级 3 目标：**将 PLC 设备协议以 Tier1 ALC 插件形态落地，消除根目录"空壳"协议类**（当前 `Modbus.cs / Melsec.cs / Fatek.cs / Fuji.cs / Keyence.cs / BACnet.cs / LoRa.cs` 及其 `*Test.cs` 仅为占位，未实现），使协议驱动可热插拔、隔离加载、按设备类型动态发现。

## 1. 设计

### 1.1 信任边界（复用 ADR-115 两层）
- **Tier1 = ALC 插件**（不可信 / 设备协议 / 可热重载）：每个协议一个独立程序集，经 `PluginLoadContext` 隔离加载，互不污染、可独立升级。
- 共享契约程序集 `Plc.Plugins.Contracts`（`Private=false`）继续承担类型身份统一角色——新增的设备协议契约放入此共享程序集，避免插件与宿主间类型分裂。

### 1.2 新增契约（置于 `Plc.Plugins.Contracts`）
```csharp
// 设备协议抽象：一个插件可实现一个或多个协议
public interface IDeviceProtocol
{
    string ProtocolId { get; }          // 如 "modbus.tcp"
    string DisplayName { get; }
    DeviceCapabilities Capabilities { get; }
    IDeviceSession CreateSession(DeviceConnectionOptions options);
}

// 设备会话：一次连接的生命周期，按寄存器类型读写
public interface IDeviceSession : IAsyncDisposable
{
    Task<byte[]> ReadAsync(ReadRequest request, CancellationToken ct);
    Task WriteAsync(WriteRequest request, CancellationToken ct);
}

// 宿主注册中心（共享接口，宿主提供实现；插件经 context.Services 解析并注册）
public interface IDeviceCatalog
{
    void Register(IDeviceProtocol protocol);
    IDeviceProtocol? Get(string protocolId);
    IReadOnlyCollection<IDeviceProtocol> All { get; }
}
// 值类型：DeviceConnectionOptions / ReadRequest / WriteRequest / RegisterType / DeviceCapabilities
```

### 1.3 宿主侧落地
- `DeviceCatalog : IDeviceCatalog` 以单例注册进 DI（宿主内部实现）。
- 插件在 `IPlugin.StartAsync(ctx)` 内通过 `ctx.Services.GetRequiredService<IDeviceCatalog>().Register(this)` 自注册（插件同时实现 `IPlugin` 与 `IDeviceProtocol`）。
- `DeviceProtocolsModule : ICapabilityModule + ICarterModule`：暴露
  - `GET /api/devices/protocols` —— 列出已注册协议（供 HMI/AIOT 发现可用协议）；
  - `GET/POST /api/devices/{protocolId}/read|write` —— 网关式透传（便于前端/HMI 经宿主访问现场设备）。

### 1.4 参考实现：ModbusTcpDevicePlugin
- 将现有 `DemoModbusPlugin` 升级为**真实 Modbus TCP 协议插件**：自实现 MBAP 头 + PDU 帧（读保持寄存器 `0x03`、写单寄存器 `0x06`、写多寄存器 `0x10`、写多线圈 `0x0F`），基于 `System.Net.Sockets` TCP，**无额外依赖**。
- 启动即注册到 `DeviceCatalog`；`/api/devices/protocols` 应返回 `modbus.tcp`。

## 2. 迁移计划（消除根目录空壳）

按协议逐个转为 `plc-host/plugins/` 下独立 ALC 插件；每个插件迁移完成后，从根目录删除对应空壳 `.cs` 与 `*Test.cs`。

| 现有空壳 | 目标插件 | 顺序 |
|----------|----------|------|
| `Modbus.cs` | `ModbusTcpDevicePlugin`（参考实现，首做） | 1 |
| `Melsec.cs` | `MelsecMCDevicePlugin` | 2 |
| `Fatek.cs` | `FatekDevicePlugin` | 3 |
| `Fuji.cs` | `FujiDevicePlugin` | 4 |
| `Keyence.cs` | `KeyenceDevicePlugin` | 5 |
| `BACnet.cs` | `BacnetDevicePlugin` | 6 |
| `LoRa.cs` | `LoRaDevicePlugin` | 7 |

## 3. Native AOT 兼容性提示

设备协议插件依赖 ALC 动态加载与反射，与 Native AOT 修剪根本冲突（详见 ADR-117）。
**结论**：协议插件以"解释执行 + ALC 隔离"为主，**不纳入宿主 AOT 裁剪图**；若未来宿主核心走 AOT，仅对插件加载入口做 `[DynamicDependency]` 标注，协议实现本身保持 JIT/ALC。

## 4. 风险与跟进

- **协议实现复杂度**：BACnet / Melsec MC 帧较复杂，逐个推进，参考实现（Modbus TCP）先行。
- **根目录空壳引用关系**：迁移前需确认 `Feature.csproj`（前端工程）是否编译引用这些根目录 `.cs`；若有引用须先解耦再删除。
- **ALC + AOT 冲突**：见 ADR-117，已在 §3 明确边界。

## 5. 完成判据（P3 收口）

- `IDeviceProtocol / IDeviceSession / IDeviceCatalog` 契约 + `DeviceCatalog` 实现落地；
- `ModbusTcpDevicePlugin` 真实读/写（MBAP+PDU）跑通；
- `GET /api/devices/protocols` 返回 `modbus.tcp`；端到端验证通过；
- 根目录 `Modbus.cs` 等空壳的迁移/删除计划明确并在后续提交中执行。
