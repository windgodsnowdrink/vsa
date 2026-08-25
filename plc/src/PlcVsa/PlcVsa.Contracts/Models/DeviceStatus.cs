namespace PlcVsa.Contracts.Models;

/// <summary>
/// Runtime connectivity and health status for a PLC / edge device.
/// </summary>
public sealed class DeviceStatus
{
    public string DeviceId { get; set; } = string.Empty;

    public string Status { get; set; } = "offline";

    public DateTimeOffset LastSeen { get; set; } = DateTimeOffset.UtcNow;

    public string? FirmwareVersion { get; set; }

    public double? SignalStrength { get; set; }

    public Dictionary<string, string>? Properties { get; set; }
}
