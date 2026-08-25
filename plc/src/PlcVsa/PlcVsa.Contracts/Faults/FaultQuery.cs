namespace PlcVsa.Contracts.Faults;

/// <summary>多维故障查询条件（§1.13.5）。</summary>
public sealed record FaultQuery
{
    public string? DeviceId { get; init; }
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
    public FaultSeverity? MinSeverity { get; init; }
    public string? Code { get; init; }
    public FaultStatus? Status { get; init; }
    public string? Fingerprint { get; init; }        // 查同类故障聚合
}
