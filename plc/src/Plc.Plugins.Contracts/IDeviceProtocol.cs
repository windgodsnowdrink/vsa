namespace Plc.Plugins.Contracts;

/// <summary>寄存器 / 线圈类型（对应 Modbus 功能码语义）。</summary>
public enum RegisterType
{
    /// <summary>线圈（读 0x01 / 写 0x05/0x0F）。</summary>
    Coil = 1,

    /// <summary>离散输入（读 0x02）。</summary>
    DiscreteInput = 2,

    /// <summary>保持寄存器（读 0x03 / 写 0x06/0x10）。</summary>
    HoldingRegister = 3,

    /// <summary>输入寄存器（读 0x04）。</summary>
    InputRegister = 4,
}

/// <summary>设备连接参数。</summary>
public sealed record DeviceConnectionOptions(
    string Host,
    int Port,
    byte UnitId = 1,
    int TimeoutMs = 3000);

/// <summary>读请求。</summary>
public sealed record ReadRequest(
    ushort StartAddress,
    ushort Count,
    RegisterType Type);

/// <summary>写请求（单值或多值）。</summary>
public sealed record WriteRequest(
    ushort StartAddress,
    RegisterType Type,
    ushort[] Values);

/// <summary>协议能力声明。</summary>
public sealed record DeviceCapabilities(
    bool SupportsRead,
    bool SupportsWrite,
    RegisterType[] SupportedTypes);

/// <summary>
/// 设备协议抽象（Tier1 ALC 插件实现）。一个插件可实现一个或多个协议。
/// 宿主经共享契约（ALC 共享程序集）加载；插件在 <see cref="IPlugin.StartAsync"/> 内自注册到 <see cref="IDeviceCatalog"/>。
/// </summary>
public interface IDeviceProtocol
{
    /// <summary>协议唯一标识，如 "modbus.tcp"。</summary>
    string ProtocolId { get; }

    /// <summary>展示名。</summary>
    string DisplayName { get; }

    /// <summary>能力声明。</summary>
    DeviceCapabilities Capabilities { get; }

    /// <summary>基于连接参数创建一次设备会话。</summary>
    IDeviceSession CreateSession(DeviceConnectionOptions options);
}

/// <summary>一次设备连接的生命周期会话：按寄存器类型读 / 写原始字节。</summary>
public interface IDeviceSession : IAsyncDisposable
{
    /// <summary>读取原始字节（协议相关：如 Modbus 为寄存器值大端排列）。</summary>
    Task<byte[]> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default);

    /// <summary>写入值（values 含义与协议相关）。</summary>
    Task WriteAsync(WriteRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// 宿主设备协议注册中心（共享接口，宿主提供实现）。
/// 插件经 <see cref="IPluginContext.Services"/> 解析并注册自身协议，HMI / AIOT 经此发现可用协议。
/// </summary>
public interface IDeviceCatalog
{
    /// <summary>注册一个设备协议实现。</summary>
    void Register(IDeviceProtocol protocol);

    /// <summary>按协议标识获取协议，未找到返回 null。</summary>
    IDeviceProtocol? Get(string protocolId);

    /// <summary>已注册协议快照。</summary>
    IReadOnlyCollection<IDeviceProtocol> All { get; }
}
