using System.Collections.Concurrent;
using PlcAiot.Abstractions.Faults;

namespace PlcAiot.Faults;

/// <summary>热存储（内存）：适合未结/近期故障的实时查询；进程重启后丢失（生产用 PG 替代）。</summary>
public sealed class InMemoryFaultStore : IFaultStore
{
    private readonly ConcurrentDictionary<Guid, FaultEvent> _store = new();

    public Task AddAsync(FaultEvent fault, CancellationToken ct = default)
    {
        _store[fault.FaultId] = fault;                  // 同 id 覆盖 → 状态更新（Open→AutoResolved）
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<FaultEvent>> QueryAsync(FaultQuery q, CancellationToken ct = default)
    {
        var result = _store.Values.AsEnumerable().Where(Matches(q)).OrderBy(f => f.OccurredAt).ToList();
        return Task.FromResult((IReadOnlyList<FaultEvent>)result);
    }

    internal static Func<FaultEvent, bool> Matches(FaultQuery q) => f =>
        (q.DeviceId is null || f.DeviceId == q.DeviceId) &&
        (q.Code is null || f.Code == q.Code) &&
        (q.Status is null || f.Status == q.Status) &&
        (q.Fingerprint is null || f.Fingerprint == q.Fingerprint) &&
        (q.MinSeverity is null || f.Severity >= q.MinSeverity) &&
        (q.From is null || f.OccurredAt >= q.From) &&
        (q.To is null || f.OccurredAt <= q.To);
}
