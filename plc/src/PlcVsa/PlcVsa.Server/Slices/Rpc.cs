// ────────────────────────────────────────────────────────────────────────────
// Slice: Rpc（HostRpcServer + StreamJsonRpc Attach/监听）
// 对应 8 项需求 §7 vs-streamjsonrpc 组件间 RPC
// ────────────────────────────────────────────────────────────────────────────
using SJR = StreamJsonRpc;
using PlcVsa.Contracts.Devices;
using PlcVsa.Server.Infrastructure;

namespace PlcVsa.Server.Slices;

public interface IHostRpc
{
    Task<string> GetProtocolVersionAsync(string protocolId);
    Task<bool> PublishProtocolFrameAsync(string protocol, string device, string payloadBase64);
}

public interface IPluginRpc
{
    Task<int> GetDeviceCountAsync();
    Task<string> ExecuteAddressReadAsync(string address, string dataType);
}

public interface IHostRpcServer : IHostRpc, global::System.IAsyncDisposable
{
    void Attach(Stream stream);
}

public sealed class HostRpcServer : IHostRpcServer
{
    private readonly IDeviceCatalog _catalog;
    private readonly BusProducer _bus;
    private SJR.JsonRpc? _rpc;
    public HostRpcServer(IDeviceCatalog c, BusProducer b) { _catalog = c; _bus = b; }

    public void Attach(Stream stream)
    {
        _rpc = SJR.JsonRpc.Attach(stream);
        _rpc.AddLocalRpcTarget((IHostRpc)this);
        _rpc.StartListening();
    }

    public System.Threading.Tasks.ValueTask DisposeAsync()
    {
        if (_rpc is not null)
        {
            _rpc.Dispose();
            _rpc = null;
        }
        return default;
    }

    public Task<string> GetProtocolVersionAsync(string protocolId)
    {
        var p = _catalog.Get(protocolId);
        return Task.FromResult(p is null ? "unknown" : p.DisplayName);
    }

    public Task<bool> PublishProtocolFrameAsync(string protocol, string device, string payloadBase64)
    {
        try
        {
            _bus.PublishFrame(new ProtocolFrame
            {
                Protocol = protocol,
                DeviceId = device,
                Payload = Convert.FromBase64String(payloadBase64),
                Ts = DateTimeOffset.UtcNow,
            });
            return Task.FromResult(true);
        }
        catch { return Task.FromResult(false); }
    }
}
