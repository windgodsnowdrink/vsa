namespace PlcVsa.Contracts.Devices;

/// <summary>健康检查探针结果（看门狗/就绪探针用）。</summary>
public sealed record HealthProbeResult
{
    public bool Healthy { get; init; }
    public string? Detail { get; init; }
    public TimeSpan Roundtrip { get; init; }
}
