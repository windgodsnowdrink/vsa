using System.Collections.Concurrent;
using Plc.Plugins.Contracts;

namespace Plc.Host.Devices;

/// <summary>
/// 设备协议注册中心实现（宿主内部单例）。
/// 由 <see cref="Modules.DeviceProtocolsModule"/> 注册为 <see cref="IDeviceCatalog"/>，
/// 各 Tier1 设备协议插件在启动期经 <see cref="IPluginContext.Services"/> 解析并调用 <see cref="Register"/>。
/// </summary>
public sealed class DeviceCatalog : IDeviceCatalog
{
    private readonly ConcurrentDictionary<string, IDeviceProtocol> _protocols =
        new(StringComparer.OrdinalIgnoreCase);

    public void Register(IDeviceProtocol protocol)
    {
        ArgumentNullException.ThrowIfNull(protocol);
        _protocols[protocol.ProtocolId] = protocol;
    }

    public IDeviceProtocol? Get(string protocolId) =>
        _protocols.TryGetValue(protocolId, out var protocol) ? protocol : null;

    public IReadOnlyCollection<IDeviceProtocol> All => _protocols.Values.ToArray();
}
