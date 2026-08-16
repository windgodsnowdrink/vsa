using System.Text.Json;
using PlcAiot.Abstractions.Faults;

namespace PlcAiot.Faults;

/// <summary>
/// 冷存储（JSONL 文件）：演示离线可用的「时序/全量」故障流。
/// 生产环境对应 InfluxDB（PlcAiot.Faults.Persistent）。
/// </summary>
public sealed class FileFaultStore : IFaultStore
{
    private readonly string _path;
    private readonly Lock _gate = new();
    private static readonly JsonSerializerOptions Opt = new(JsonSerializerDefaults.Web);

    public FileFaultStore(string path) => _path = path;

    public Task AddAsync(FaultEvent fault, CancellationToken ct = default)
    {
        var line = JsonSerializer.Serialize(fault, Opt);
        lock (_gate) File.AppendAllText(_path, line + Environment.NewLine);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<FaultEvent>> QueryAsync(FaultQuery q, CancellationToken ct = default)
    {
        var result = new List<FaultEvent>();
        lock (_gate)
        {
            if (File.Exists(_path))
            {
                foreach (var line in File.ReadAllLines(_path))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var f = JsonSerializer.Deserialize<FaultEvent>(line, Opt);
                    if (f is not null && InMemoryFaultStore.Matches(q)(f)) result.Add(f);
                }
            }
        }
        result.Sort((a, b) => a.OccurredAt.CompareTo(b.OccurredAt));
        return Task.FromResult((IReadOnlyList<FaultEvent>)result);
    }
}
