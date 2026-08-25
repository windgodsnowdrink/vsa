namespace PlcVsa.Contracts.Devices;

/// <summary>采样质量（与 OPC-UA 质量码对齐的简化版）。</summary>
public enum PointQuality { Good, Uncertain, Bad }

/// <summary>协议无关的采样点；PollAsync 统一吐出此结构，上层只认 Tag（§1.12.2）。</summary>
public sealed record PointSample
{
    public required string Tag { get; init; }
    public double Value { get; init; }
    public PointQuality Quality { get; init; } = PointQuality.Good;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string? Unit { get; init; }
}
