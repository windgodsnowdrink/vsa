namespace PlcVsa.Contracts.Models;

/// <summary>
/// PLC / edge device telemetry sample.
/// </summary>
public sealed class DeviceTelemetry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DeviceId { get; set; } = string.Empty;

    public string? MetricName { get; set; }

    public double Value { get; set; }

    public string? Unit { get; set; }

    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    public Dictionary<string, string>? Tags { get; set; }
}
