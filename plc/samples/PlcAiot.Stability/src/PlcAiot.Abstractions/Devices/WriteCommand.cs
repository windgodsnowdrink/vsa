namespace PlcAiot.Abstractions.Devices;

/// <summary>南向写指令（强一致，§1.7）。</summary>
public sealed record WriteCommand
{
    public required string Tag { get; init; }
    public required double Value { get; init; }
    public string? CorrelationId { get; init; }       // 全链路关联（§1.13.2）
}
