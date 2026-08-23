using PlcAiot.Abstractions.Devices;

namespace PlcAiot.Devices;

/// <summary>设备注册表契约：新增/更换 PLC = 注册 Profile；换传输 = 更新 Profile 指针（§1.12.4）。</summary>
public interface IDeviceCatalog
{
    void RegisterProfile(DeviceProfile profile);
    void SwitchTransport(string deviceId, string transport);   // 换传输(TCP↔RTU)不改点表
    IDeviceProtocol Resolve(string deviceId);                  // 按 Profile 解析出协议实现
    IReadOnlyCollection<DeviceProfile> All { get; }
}

/// <summary>
/// 设备注册表实现。换型流程：catalog.RegisterProfile(s7Profile) → supervisor 重启该会话 → 上层照常按 Tag 读写。
/// </summary>
public sealed class DeviceCatalog : IDeviceCatalog
{
    private readonly Dictionary<string, DeviceProfile> _profiles = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Func<DeviceProfile, IDeviceProtocol>> _factories = new(StringComparer.OrdinalIgnoreCase);
    private readonly Lock _gate = new();

    public DeviceCatalog()
    {
        // 默认工厂：演示用 Modbus/S7 都映射到内置模拟协议；生产替换为真实驱动插件
        RegisterFactory("Modbus", p => new SimulatedModbusProtocol(p));
        RegisterFactory("S7", p => new SimulatedModbusProtocol(p));
        RegisterFactory("OpcUa", p => new SimulatedModbusProtocol(p));
    }

    public void RegisterFactory(string protocol, Func<DeviceProfile, IDeviceProtocol> factory)
    {
        lock (_gate) _factories[protocol] = factory;
    }

    public void RegisterProfile(DeviceProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        lock (_gate) _profiles[profile.DeviceId] = profile;
    }

    public void SwitchTransport(string deviceId, string transport)
    {
        lock (_gate)
        {
            if (_profiles.TryGetValue(deviceId, out var p))
                _profiles[deviceId] = p with { Transport = transport };
        }
    }

    public IDeviceProtocol Resolve(string deviceId)
    {
        DeviceProfile profile;
        Func<DeviceProfile, IDeviceProtocol> factory;
        lock (_gate)
        {
            if (!_profiles.TryGetValue(deviceId, out profile!))
                throw new KeyNotFoundException($"No profile registered for {deviceId}");
            if (!_factories.TryGetValue(profile.Protocol, out factory!))
                throw new NotSupportedException($"No protocol factory registered for {profile.Protocol}");
        }
        return factory(profile);
    }

    public IReadOnlyCollection<DeviceProfile> All
    {
        get { lock (_gate) return _profiles.Values.ToList(); }
    }
}
