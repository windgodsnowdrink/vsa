using PlcAiot.Abstractions.Faults;

namespace PlcAiot.Faults;

/// <summary>故障解决状态机（§1.13.4）：非法流转直接抛错，保证流程可控。</summary>
public static class ResolutionStateMachine
{
    public static bool CanTransition(FaultStatus from, FaultStatus to) => (from, to) switch
    {
        (FaultStatus.Open, FaultStatus.Acknowledged) => true,
        (FaultStatus.Open, FaultStatus.AutoResolved) => true,
        (FaultStatus.Acknowledged, FaultStatus.InProgress) => true,
        (FaultStatus.InProgress, FaultStatus.Resolved) => true,
        (FaultStatus.Resolved, FaultStatus.Closed) => true,
        (FaultStatus.AutoResolved, FaultStatus.Closed) => true,
        _ => false
    };

    public static FaultStatus Transition(FaultStatus from, FaultStatus to)
    {
        if (!CanTransition(from, to))
            throw new InvalidOperationException($"illegal fault transition {from} -> {to}");
        return to;
    }
}
