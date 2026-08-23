using Npgsql;
using PlcAiot.Abstractions.Faults;
using PlcAiot.Abstractions.Logging;

namespace PlcAiot.Faults.Persistent;

/// <summary>
/// 热存储：PostgreSQL 实现（关系存未结/近期故障，支持多维查询与状态更新）。
/// 表结构：CREATE TABLE faults (fault_id uuid PRIMARY KEY, device_id text, code text, severity text,
/// category text, source text, correlation_id text, fingerprint text, occurred_at timestamptz,
/// payload text, status text);
/// </summary>
public sealed class PostgreSqlFaultStore : IFaultStore, IAsyncDisposable
{
    private readonly string _cs;
    private readonly ILogger _log;

    public PostgreSqlFaultStore(string connectionString, ILogger? log = null)
    {
        _cs = connectionString;
        _log = log ?? new ConsoleLogger("store:pg");
    }

    public async Task AddAsync(FaultEvent f, CancellationToken ct = default)
    {
        await using var conn = new NpgsqlConnection(_cs);
        await conn.OpenAsync(ct);
        await using var cmd = new NpgsqlCommand(@"
            INSERT INTO faults (fault_id,device_id,code,severity,category,source,correlation_id,fingerprint,occurred_at,payload,status)
            VALUES (@id,@dev,@code,@sev,@cat,@src,@corr,@fp,@at,@payload,@status)
            ON CONFLICT (fault_id) DO UPDATE SET status = EXCLUDED.status, payload = EXCLUDED.payload;",
            conn);
        cmd.Parameters.AddWithValue("id", f.FaultId);
        cmd.Parameters.AddWithValue("dev", f.DeviceId);
        cmd.Parameters.AddWithValue("code", f.Code);
        cmd.Parameters.AddWithValue("sev", f.Severity.ToString());
        cmd.Parameters.AddWithValue("cat", f.Category.ToString());
        cmd.Parameters.AddWithValue("src", f.Source);
        cmd.Parameters.AddWithValue("corr", f.CorrelationId);
        cmd.Parameters.AddWithValue("fp", f.Fingerprint);
        cmd.Parameters.AddWithValue("at", f.OccurredAt);
        cmd.Parameters.AddWithValue("payload", (object?)f.Payload ?? DBNull.Value);
        cmd.Parameters.AddWithValue("status", f.Status.ToString());
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<FaultEvent>> QueryAsync(FaultQuery q, CancellationToken ct = default)
    {
        var where = new List<string>();
        var ps = new List<NpgsqlParameter>();
        var i = 0;
        if (q.DeviceId is not null) { where.Add($"device_id = @p{i}"); ps.Add(new("p" + i++, q.DeviceId)); }
        if (q.Code is not null) { where.Add($"code = @p{i}"); ps.Add(new("p" + i++, q.Code)); }
        if (q.Status is not null) { where.Add($"status = @p{i}"); ps.Add(new("p" + i++, q.Status.ToString())); }
        if (q.Fingerprint is not null) { where.Add($"fingerprint = @p{i}"); ps.Add(new("p" + i++, q.Fingerprint)); }
        if (q.MinSeverity is not null) { where.Add($"severity >= @p{i}"); ps.Add(new("p" + i++, q.MinSeverity.ToString())); }
        if (q.From is not null) { where.Add($"occurred_at >= @p{i}"); ps.Add(new("p" + i++, q.From)); }
        if (q.To is not null) { where.Add($"occurred_at <= @p{i}"); ps.Add(new("p" + i++, q.To)); }

        var sql = "SELECT fault_id,device_id,code,severity,category,source,correlation_id,fingerprint,occurred_at,payload,status FROM faults"
                  + (where.Count > 0 ? " WHERE " + string.Join(" AND ", where) : "")
                  + " ORDER BY occurred_at";
        var result = new List<FaultEvent>();
        await using var conn = new NpgsqlConnection(_cs);
        await conn.OpenAsync(ct);
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddRange(ps.ToArray());
        await using var r = await cmd.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct)) result.Add(Map(r));
        return result;
    }

    private static FaultEvent Map(NpgsqlDataReader r) => new()
    {
        FaultId = r.GetGuid(0),
        DeviceId = r.GetString(1),
        Code = r.GetString(2),
        Severity = Enum.Parse<FaultSeverity>(r.GetString(3)),
        Category = Enum.Parse<FaultCategory>(r.GetString(4)),
        Source = r.GetString(5),
        CorrelationId = r.GetString(6),
        Fingerprint = r.GetString(7),
        OccurredAt = r.GetDateTime(8),
        Payload = r.IsDBNull(9) ? null : r.GetString(9),
        Status = Enum.Parse<FaultStatus>(r.GetString(10))
    };

    public async ValueTask DisposeAsync() => await Task.CompletedTask;
}
