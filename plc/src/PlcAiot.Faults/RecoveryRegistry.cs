using PlcAiot.Abstractions.Faults;

namespace PlcAiot.Faults;

/// <summary>恢复策略注册表：按故障码映射到一组 IRecoveryStrategy。</summary>
public sealed class RecoveryRegistry
{
    private readonly List<IRecoveryStrategy> _strategies = [];

    public void Register(IRecoveryStrategy strategy) => _strategies.Add(strategy);

    public IReadOnlyList<IRecoveryStrategy> Resolve(FaultEvent fault)
        => _strategies.Where(s => s.CanHandle(fault)).ToList();
}
