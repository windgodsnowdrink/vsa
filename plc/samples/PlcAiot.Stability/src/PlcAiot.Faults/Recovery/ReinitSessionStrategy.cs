using PlcAiot.Abstractions.Devices;
using PlcAiot.Abstractions.Faults;
using PlcAiot.Devices;
using PlcAiot.Abstractions.Logging;

namespace PlcAiot.Faults.Recovery;

/// <summary>会话过期 → 断开重初始化（§1.13.3）。</summary>
public sealed class ReinitSessionStrategy : IRecoveryStrategy
{
    private readonly IDeviceCatalog _catalog;
    private readonly ILogger _log;

    public ReinitSessionStrategy(IDeviceCatalog catalog, ILogger? log = null)
    {
        _catalog = catalog;
        _log = log ?? new ConsoleLogger("recover:reinit");
    }

    public bool CanHandle(FaultEvent f) => f.Code is "SESSION_STALE";

    public async ValueTask<RecoveryResult> ExecuteAsync(FaultEvent f, CancellationToken ct = default)
    {
        try
        {
            await using var proto = _catalog.Resolve(f.DeviceId);
            await proto.DisconnectAsync(ct);
            await proto.ConnectAsync(ct);
            return RecoveryResult.Succeeded;
        }
        catch (Exception ex)
        {
            _log.Log(LogLevel.Error, ex.Message);
            return RecoveryResult.EscalateToHuman;
        }
    }
}
