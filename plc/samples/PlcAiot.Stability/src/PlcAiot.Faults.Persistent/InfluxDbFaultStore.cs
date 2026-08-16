using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using PlcAiot.Abstractions.Faults;
using PlcAiot.Abstractions.Logging;

namespace PlcAiot.Faults.Persistent;

/// <summary>
/// 冷存储：InfluxDB 实现（时序存全量故障流，用于 MTTR/MTBF 看板与长期追溯，§1.13.6）。
/// </summary>
public sealed class InfluxDbFaultStore : IFaultStore, IAsyncDisposable
{
    private readonly InfluxDBClient _client;
    private readonly string _bucket;
    private readonly string _org;
    private readonly ILogger _log;

    public InfluxDbFaultStore(string url, string token, string org, string bucket, ILogger? log = null)
    {
        _client = new InfluxDBClient(url, token);
        _bucket = bucket;
        _org = org;
        _log = log ?? new ConsoleLogger("store:influx");
    }

    public async Task AddAsync(FaultEvent f, CancellationToken ct = default)
    {
        var point = PointData.Measurement("faults")
            .Tag("deviceId", f.DeviceId)
            .Tag("code", f.Code)
            .Tag("severity", f.Severity.ToString())
            .Tag("category", f.Category.ToString())
            .Tag("source", f.Source)
            .Tag("fingerprint", f.Fingerprint)
            .Tag("status", f.Status.ToString())
            .Field("value", 1.0)
            .Timestamp(f.OccurredAt, WritePrecision.Ns);
        await _client.GetWriteApiAsync().WritePointAsync(point, _bucket, _org, ct);
    }

    public async Task<IReadOnlyList<FaultEvent>> QueryAsync(FaultQuery q, CancellationToken ct = default)
    {
        var flux = $"from(bucket:\"{_bucket}\") |> range(start: -30d) |> filter(fn: (r) => r._measurement == \"faults\")";
        if (q.DeviceId is not null) flux += $" |> filter(fn: (r) => r.deviceId == \"{q.DeviceId}\")";
        if (q.Code is not null) flux += $" |> filter(fn: (r) => r.code == \"{q.Code}\")";

        var result = new List<FaultEvent>();
        var tables = await _client.GetQueryApi().QueryAsync(flux, _org, ct);
        foreach (var rec in tables.SelectMany(t => t.Records))
        {
            result.Add(new FaultEvent
            {
                DeviceId = rec.GetValueByKey("deviceId")?.ToString() ?? "",
                Code = rec.GetValueByKey("code")?.ToString() ?? "",
                Fingerprint = rec.GetValueByKey("fingerprint")?.ToString() ?? "",
                OccurredAt = rec.GetTime().HasValue ? rec.GetTime()!.Value.ToDateTimeUtc() : DateTime.UtcNow,
                Severity = FaultSeverity.Info,
                Category = FaultCategory.Comm,
                Source = "influx"
            });
        }
        return result;
    }

    public ValueTask DisposeAsync()
    {
        _client.Dispose();
        return ValueTask.CompletedTask;
    }
}
