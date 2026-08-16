using PlcAiot.Abstractions.Faults;
using PlcAiot.Abstractions.Logging;

namespace PlcAiot.Faults.Recovery;

/// <summary>急停相关 → 安全态（断输出、上报告警，不盲目重试，§7.0 G10 / §1.13.3）。</summary>
public sealed class SafeStateStrategy : IRecoveryStrategy
{
    private readonly ILogger _log;
    public SafeStateStrategy(ILogger? log = null) => _log = log ?? new ConsoleLogger("recover:safe");

    public bool CanHandle(FaultEvent f) => f.Code.StartsWith("ESTOP", StringComparison.Ordinal);

    public ValueTask<RecoveryResult> ExecuteAsync(FaultEvent f, CancellationToken ct = default)
    {
        _log.Log(LogLevel.Critical, $"SAFE STATE engaged for {f.DeviceId}: {f.Code}");
        return ValueTask.FromResult(RecoveryResult.Succeeded);
    }
}
