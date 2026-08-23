namespace PlcAiot.Supervisor;

/// <summary>单设备会话的监督参数（§1.11.2 / §1.11.3）。</summary>
public sealed record DeviceOptions
{
    public string DeviceId { get; init; } = "";
    public int MaxRestarts { get; init; } = 10;                 // 重启预算（防 flapping 雪崩）
    public int WatchdogMisses { get; init; } = 3;              // 连续丢失心跳数 → 判定失联
    public TimeSpan WatchdogTimeout { get; init; } = TimeSpan.FromSeconds(5);
    public TimeSpan MaxBackoff { get; init; } = TimeSpan.FromMinutes(5);
}
