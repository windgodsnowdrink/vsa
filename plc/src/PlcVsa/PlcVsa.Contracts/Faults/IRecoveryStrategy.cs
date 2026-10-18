namespace PlcVsa.Contracts.Faults;

/// <summary>自动化恢复执行结果。</summary>
public enum RecoveryResult { Succeeded, Failed, EscalateToHuman, Deferred }

/// <summary>故障码 → 恢复策略 的映射契约（§1.13.3）。</summary>
public interface IRecoveryStrategy
{
    bool CanHandle(FaultEvent fault);
    ValueTask<RecoveryResult> ExecuteAsync(FaultEvent fault, CancellationToken ct = default);
}
