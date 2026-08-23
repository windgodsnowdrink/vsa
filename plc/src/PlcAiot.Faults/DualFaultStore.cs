using PlcAiot.Abstractions.Faults;
using PlcAiot.Abstractions.Logging;

namespace PlcAiot.Faults;

/// <summary>
/// 双写存储（热 + 冷）：AddAsync 先写热库，再写冷库（失败仅告警不阻断）。
/// 生产示例：<c>new DualFaultStore(new PostgreSqlFaultStore(cs), new InfluxDbFaultStore(...))</c>。
/// </summary>
public sealed class DualFaultStore : IFaultStore, IAsyncDisposable
{
    private readonly IFaultStore _primary;
    private readonly IFaultStore _secondary;
    private readonly ILogger _log;

    public DualFaultStore(IFaultStore primary, IFaultStore secondary, ILogger? log = null)
    {
        _primary = primary;
        _secondary = secondary;
        _log = log ?? new ConsoleLogger("store:dual");
    }

    public async Task AddAsync(FaultEvent fault, CancellationToken ct = default)
    {
        await _primary.AddAsync(fault, ct);                 // 热：PG / 内存（查询走这里）
        try
        {
            await _secondary.AddAsync(fault, ct);           // 冷：InfluxDB / 文件
        }
        catch (Exception ex)
        {
            _log.Log(LogLevel.Warning, $"secondary store write failed (fault {fault.FaultId}): {ex.Message}");
        }
    }

    public Task<IReadOnlyList<FaultEvent>> QueryAsync(FaultQuery q, CancellationToken ct = default)
        => _primary.QueryAsync(q, ct);

    public async ValueTask DisposeAsync()
    {
        if (_primary is IAsyncDisposable ap) await ap.DisposeAsync();
        if (_secondary is IAsyncDisposable asd) await asd.DisposeAsync();
    }
}
