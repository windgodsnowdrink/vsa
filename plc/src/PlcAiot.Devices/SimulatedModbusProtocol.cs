using System.Runtime.CompilerServices;
using PlcAiot.Abstractions.Devices;
using PlcAiot.Abstractions.Logging;

namespace PlcAiot.Devices;

/// <summary>
/// 内置模拟协议实现（演示用）。真实环境替换为 Modbus/Net/S7netplus/OPC UA 等驱动，
/// 只要实现 IDeviceProtocol，上层零改动（§1.12.2 / §1.12.6）。
/// </summary>
public sealed class SimulatedModbusProtocol : IDeviceProtocol
{
    private readonly DeviceProfile _profile;
    private readonly ILogger _log;
    private bool _connected;
    private int _tick;
    private readonly Random _rng = new();

    public SimulatedModbusProtocol(DeviceProfile profile, ILogger? logger = null)
    {
        _profile = profile;
        _log = logger ?? new ConsoleLogger($"proto:{profile.DeviceId}");
    }

    public string ProtocolId => _profile.Protocol;

    public DeviceCapabilities Capabilities => new()
    {
        SupportsWrite = true,
        SupportsSubscribe = false,                       // 模拟协议不支持订阅 → 上层退化为轮询
        MaxRegistersPerFrame = 120,
        NativeDataTypes = new HashSet<string> { "bool", "int16", "uint16", "float" },
        MaxConcurrentSessions = 1
    };

    public ValueTask ConfigureAsync(DeviceProfile profile, CancellationToken ct) => ValueTask.CompletedTask;

    public async ValueTask ConnectAsync(CancellationToken ct)
    {
        _log.Log(LogLevel.Information,
            $"connect {_profile.Protocol}/{_profile.Transport} -> {_profile.Endpoint.Host}:{_profile.Endpoint.Port}");
        await Task.Delay(50, ct);                        // 模拟握手
        _connected = true;
    }

    public ValueTask DisconnectAsync(CancellationToken ct)
    {
        _connected = false;
        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<PointSample> PollAsync(
        [EnumeratorCancellation] CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (!_connected) yield break;
            foreach (var pt in _profile.Points)
            {
                var raw = Math.Sin(_tick / 10.0 + pt.Address) * 50 + 50;     // 演示波形
                yield return new PointSample
                {
                    Tag = pt.Tag,
                    Value = raw * pt.Scale + pt.Offset,
                    Quality = PointQuality.Good,
                    Timestamp = DateTime.UtcNow,
                    Unit = pt.Unit
                };
            }
            _tick++;
            await Task.Delay(Math.Max(50, _profile.PollingMs / 10), ct);
        }
    }

    public ValueTask WriteAsync(WriteCommand cmd, CancellationToken ct)
    {
        if (!_connected) throw new InvalidOperationException("device not connected");
        _log.Log(LogLevel.Debug, $"write {cmd.Tag}={cmd.Value} (corr={cmd.CorrelationId})");
        return ValueTask.CompletedTask;
    }

    public ValueTask<HealthProbeResult> HealthProbeAsync(CancellationToken ct)
        => new(new HealthProbeResult
        {
            Healthy = _connected,
            Detail = _connected ? "ok" : "down",
            Roundtrip = TimeSpan.FromMilliseconds(1)
        });

    public ValueTask DisposeAsync()
    {
        if (_connected) DisconnectAsync(CancellationToken.None).GetAwaiter().GetResult();
        return ValueTask.CompletedTask;
    }
}
