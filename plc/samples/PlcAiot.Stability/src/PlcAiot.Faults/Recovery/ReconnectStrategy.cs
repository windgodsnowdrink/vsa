using PlcAiot.Abstractions.Devices;
using PlcAiot.Abstractions.Faults;
using PlcAiot.Devices;
using PlcAiot.Abstractions.Logging;

namespace PlcAiot.Faults.Recovery;

/// <summary>断线/会话过期 → 重连（指数退避，最多 5 次，失败升级人工，§1.13.3）。</summary>
public sealed class ReconnectStrategy : IRecoveryStrategy
{
    private readonly IDeviceCatalog _catalog;
    private readonly ILogger _log;

    public ReconnectStrategy(IDeviceCatalog catalog, ILogger? log = null)
    {
        _catalog = catalog;
        _log = log ?? new ConsoleLogger("recover:reconnect");
    }

    public bool CanHandle(FaultEvent f) => f.Code is "COMM_TIMEOUT" or "SESSION_STALE";

    public async ValueTask<RecoveryResult> ExecuteAsync(FaultEvent f, CancellationToken ct = default)
    {
        for (var i = 1; i <= 5; i++)
        {
            try
            {
                await using var proto = _catalog.Resolve(f.DeviceId);
                await proto.ConnectAsync(ct);
                _log.Log(LogLevel.Information, $"reconnected {f.DeviceId} (attempt {i})");
                return RecoveryResult.Succeeded;
            }
            catch (Exception ex) when (i < 5)
            {
                _log.Log(LogLevel.Warning, $"reconnect attempt {i} failed: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, i))), ct);
            }
        }
        return RecoveryResult.EscalateToHuman;
    }
}
