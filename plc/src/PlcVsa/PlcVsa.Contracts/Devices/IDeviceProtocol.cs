using System.Threading.Tasks;
namespace PlcVsa.Contracts.Devices;

/// <summary>
/// 设备协议抽象（Tier1 ALC 插件实现）。一个插件可实现一个或多个协议。
/// 宿主经共享契约（ALC 共享程序集）加载；插件在 IPlugin.StartAsync 内自注册到 IDeviceCatalog。
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
