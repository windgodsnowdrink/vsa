using PlcAiot.Abstractions.Faults;
using PlcAiot.Abstractions.Logging;

namespace PlcAiot.Faults;

/// <summary>
/// 故障中枢：入库 + 自动恢复 + 状态流转（§1.13）。
/// 自动化恢复失败 → EscalateToHuman（通知/工单，§1.13.4）。
/// </summary>
public sealed class FaultService
{
    private readonly IFaultStore _store;
    private readonly RecoveryRegistry _recovery;
    private readonly ILogger _log;

    public FaultService(IFaultStore store, RecoveryRegistry recovery, ILogger? log = null)
    {
        _store = store;
        _recovery = recovery;
        _log = log ?? new ConsoleLogger("fault:svc");
    }

    public async Task RaiseAsync(FaultEvent fault, CancellationToken ct = default)
    {
        await _store.AddAsync(fault, ct);
        _log.Log(LevelFor(fault.Severity), $"FAULT {fault.Code} on {fault.DeviceId} [{fault.Fingerprint}]");

        foreach (var strategy in _recovery.Resolve(fault))
        {
            var result = await strategy.ExecuteAsync(fault, ct);
            if (result == RecoveryResult.Succeeded)
            {
                var resolved = fault with { Status = FaultStatus.AutoResolved };
                await _store.AddAsync(resolved, ct);        // 同 id 覆盖状态（热库生效）
                _log.Log(LogLevel.Information, $"auto-recovered {fault.FaultId} via {strategy.GetType().Name}");
                return;
            }
            if (result == RecoveryResult.EscalateToHuman)
            {
                _log.Log(LogLevel.Critical, $"escalate to human: {fault.FaultId}");
                return;
            }
        }
    }

    private static LogLevel LevelFor(FaultSeverity s) => s switch
    {
        FaultSeverity.Critical => LogLevel.Critical,
        FaultSeverity.Error => LogLevel.Error,
        FaultSeverity.Warning => LogLevel.Warning,
        _ => LogLevel.Information
    };
}
