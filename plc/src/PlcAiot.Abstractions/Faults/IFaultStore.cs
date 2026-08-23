namespace PlcAiot.Abstractions.Faults;

/// <summary>故障存储抽象：支持冷热分层（热=PG/内存关系；冷=InfluxDB/文件时序）。</summary>
public interface IFaultStore
{
    Task AddAsync(FaultEvent fault, CancellationToken ct = default);
    Task<IReadOnlyList<FaultEvent>> QueryAsync(FaultQuery q, CancellationToken ct = default);
}
