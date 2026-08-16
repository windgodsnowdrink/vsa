using PlcAiot.Abstractions.Faults;
using PlcAiot.Abstractions.Logging;

namespace PlcAiot.Faults.Recovery;

/// <summary>边缘掉线 → 故障转移（active-standby 切换，§1.11.6 / §1.13.3）。</summary>
public sealed class FailoverStrategy : IRecoveryStrategy
{
    private readonly ILogger _log;
    public FailoverStrategy(ILogger? log = null) => _log = log ?? new ConsoleLogger("recover:failover");

    public bool CanHandle(FaultEvent f) => f.Code is "EDGE_DOWN";
    public ValueTask<RecoveryResult> ExecuteAsync(FaultEvent f, CancellationToken ct = default)
    {
        _log.Log(LogLevel.Warning, $"failover triggered for edge of {f.DeviceId}");
        return ValueTask.FromResult(RecoveryResult.Deferred);
    }
}
