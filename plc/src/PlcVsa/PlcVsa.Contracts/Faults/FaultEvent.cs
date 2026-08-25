namespace PlcVsa.Contracts.Faults;

/// <summary>
/// 统一故障事件：带指纹去重 + 关联 ID 串联全链路（采集→处理→指令→存储）。
/// 所有异常（断线、校验失败、写失败、急停、超阈值）统一为此模型（§1.13.1）。
/// </summary>
public sealed record FaultEvent
{
    public Guid FaultId { get; init; } = Guid.NewGuid();
    public string DeviceId { get; init; } = "";
    public string Code { get; init; } = "";                 // 标准化故障码（如 COMM_TIMEOUT）
    public FaultSeverity Severity { get; init; }             // Info/Warning/Error/Critical
    public FaultCategory Category { get; init; }             // Comm/Protocol/Safety/Rule/...
    public string Source { get; init; } = "";                // 协议插件/规则引擎/...
    public string CorrelationId { get; init; } = "";         // 全链路关联
    public string Fingerprint { get; init; } = "";           // 设备+码+短时窗 去重聚合
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string? Payload { get; init; }                    // 故障时刻设备状态快照
    public FaultStatus Status { get; init; } = FaultStatus.Open;
}
