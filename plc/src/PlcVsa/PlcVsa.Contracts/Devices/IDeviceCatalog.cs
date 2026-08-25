namespace PlcVsa.Contracts.Devices;

/// <summary>
/// 宿主设备协议注册中心（共享接口，宿主实现）。
/// 插件经 IPluginContext.Services 解析并自注册，Server/AIOT 经此发现可用协议。
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
