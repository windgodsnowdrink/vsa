// slices/ingest.http.cs — MQTT 摄取主路径端点（ADR-113 ① EMQX 规则引擎 → HTTPS）
// EMQX 规则引擎将设备主题转发到此；请求经共享密钥 + X-Tenant-Id 校验（server-to-server，非用户 JWT）。
// 契约（EMQX HTTP Action 模板见 docs/ADR-113-implementation.md）：
//   POST /api/v1/ingest/telemetry
//     Headers: X-Tenant-Id: <tid(guid)>  X-Ingest-Key: <shared secret>
//     Body:    { "deviceId": "<guid>", "ts": <unix秒,可选>, "metrics": { "temp": 23.5, ... } }
//   POST /api/v1/ingest/fault
//     Body:    { "deviceId": "<guid>", "code": "E001", "severity": "high", "message": "..." }
// 两条路径均复用 Ingest*Command（与 MqttBridgeService 完全一致）。
#include "../infra.cs"
using System.Text.Json;

public static class IngestApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapPost("/ingest/telemetry", static async (HttpContext ctx, IMediator m, IConfiguration cfg) =>
        {
            var (tid, err) = IngestAuth.Authenticate(ctx, cfg);
            if (err is not null) return err;
            var body = await ctx.Request.ReadFromJsonAsync<TelemetryIngestRequest>(ctx.RequestAborted);
            if (body is null) return Api.Fail(40000, "请求体为空");
            if (!IngestAuth.TryDeviceId(body.DeviceId, body.Topic, out var deviceId))
                return Api.Fail(40000, "缺少或非法的 deviceId（或无法从 topic 解析）");
            var (ts, metrics) = IngestAuth.NormalizeTelemetry(body.Ts, body.Metrics);
            return await EndpointHelpers.Run(async () =>
                Api.Ok(await m.Send(new IngestTelemetryCommand(tid, deviceId, ts, metrics, metrics.Count))));
        }).AllowAnonymous();

        api.MapPost("/ingest/fault", static async (HttpContext ctx, IMediator m, IConfiguration cfg) =>
        {
            var (tid, err) = IngestAuth.Authenticate(ctx, cfg);
            if (err is not null) return err;
            var body = await ctx.Request.ReadFromJsonAsync<FaultIngestRequest>(ctx.RequestAborted);
            if (body is null) return Api.Fail(40000, "请求体为空");
            if (!IngestAuth.TryDeviceId(body.DeviceId, body.Topic, out var deviceId))
                return Api.Fail(40000, "缺少或非法的 deviceId（或无法从 topic 解析）");
            var code = string.IsNullOrWhiteSpace(body.Code) ? "UNKNOWN" : body.Code!;
            var severity = string.IsNullOrWhiteSpace(body.Severity) ? "medium" : body.Severity!;
            return await EndpointHelpers.Run(async () =>
                Api.Ok(await m.Send(new IngestFaultCommand(tid, deviceId, code, severity, body.Message))));
        }).AllowAnonymous();
    }
}

public sealed record TelemetryIngestRequest(string? DeviceId, string? Topic, long? Ts, Dictionary<string, double>? Metrics);
public sealed record FaultIngestRequest(string? DeviceId, string? Topic, string? Code, string? Severity, string? Message);

// 摄取鉴权与归一化（共享密钥 + 租户头；topic 兜底解析 deviceId）
internal static class IngestAuth
{
    public static (Guid Tid, IResult? Error) Authenticate(HttpContext ctx, IConfiguration cfg)
    {
        var sharedKey = cfg["Ingest:SharedKey"];
        if (string.IsNullOrWhiteSpace(sharedKey))
            return (Guid.Empty, Api.Fail(50300, "摄取未配置（缺少 Ingest:SharedKey）"));

        if (!ctx.Request.Headers.TryGetValue("X-Ingest-Key", out var key) || key != sharedKey)
            return (Guid.Empty, Api.Fail(40100, "非法的摄取密钥"));

        if (!ctx.Request.Headers.TryGetValue("X-Tenant-Id", out var tidHeader) ||
            !Guid.TryParse(tidHeader.ToString(), out var tid) || tid == Guid.Empty)
            return (Guid.Empty, Api.Fail(40300, "缺少或非法的 X-Tenant-Id"));

        return (tid, null);
    }

    public static bool TryDeviceId(string? deviceId, string? topic, out Guid result)
    {
        result = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(deviceId) && Guid.TryParse(deviceId, out result))
            return true;
        if (!string.IsNullOrWhiteSpace(topic))
        {
            var segs = topic.Split('/');
            if (segs.Length == 5 && segs[0] == "tenants" && segs[2] == "devices"
                && Guid.TryParse(segs[3], out result))
                return true;
        }
        return false;
    }

    public static (DateTime ts, IReadOnlyDictionary<string, double> metrics) NormalizeTelemetry(long? ts, Dictionary<string, double>? metrics)
    {
        var time = ts is { } unix && unix > 0
            ? DateTimeOffset.FromUnixTimeSeconds(unix).UtcDateTime
            : DateTime.UtcNow;
        var dict = (IReadOnlyDictionary<string, double>)(metrics ?? new Dictionary<string, double>());
        return (time, dict);
    }
}
