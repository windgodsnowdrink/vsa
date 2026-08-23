namespace PlcAiot.Shared.Models;

/// <summary>
/// Alert / alarm raised by a device, rule engine, or the platform.
/// </summary>
public sealed class Alert
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DeviceId { get; set; } = string.Empty;

    public string Severity { get; set; } = "warning";

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool Acknowledged { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ResolvedAt { get; set; }

    public Dictionary<string, string>? Metadata { get; set; }
}
