using Plc.Plugins.Contracts;

namespace PlcGateway.Protocols;

/// <summary>内置内存模拟协议（保底可用，无需真实设备即可跑通流水线 / RPC）。</summary>
public sealed class SimProtocol : IDeviceProtocol
{
    public string ProtocolId => "builtin.sim";
    public string DisplayName => "Built-in Simulator";
    public DeviceCapabilities Capabilities => new(true, true,
        new[] { RegisterType.HoldingRegister, RegisterType.Coil });

    public IDeviceSession CreateSession(DeviceConnectionOptions options) => new SimSession(options);
}

public sealed class SimSession : IDeviceSession
{
    private readonly DeviceConnectionOptions _options;
    private int _tick;

    public SimSession(DeviceConnectionOptions options) => _options = options;

    public Task<byte[]> ReadAsync(ReadRequest request, CancellationToken cancellationToken = default)
    {
        _tick++;
        var bytes = new byte[request.Count * 2];
        for (int i = 0; i < request.Count; i++)
        {
            int v = _tick + i + request.StartAddress;
            bytes[i * 2] = (byte)(v >> 8);
            bytes[i * 2 + 1] = (byte)(v & 0xFF);
        }
        return Task.FromResult(bytes);
    }

    public Task WriteAsync(WriteRequest request, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
