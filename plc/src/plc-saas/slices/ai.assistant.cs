// slices/ai.assistant.cs — AI 故障诊断 / RAG / MCP Server（ADR-112，P1 旗舰）
//  - AiAssistantApi: POST /api/v1/ai/diagnose（需 TenantUser 鉴权，Ai:Enabled 门控）
//  - PlcMcpTools: MCP Server 工具（list_devices/get_faults/get_telemetry_summary），tid 作用域隔离
// 全部经 ICurrentTenant + EF 全局过滤器隔离；向量库默认内存兜底，Qdrant:Enabled 接真实库。
#include "../infra.cs"
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using Qdrant.Client;
using System.ComponentModel;

// ===================== 诊断端点 =====================
public static class AiAssistantApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapPost("/ai/diagnose", static async (
            DiagnoseRequest req, HttpContext http, ICurrentTenant current,
            BaseDbContext db, IChatClient chat, IFaultVectorStore vectors) =>
        await EndpointHelpers.Run(async () =>
        {
            if (current.TenantId == Guid.Empty)
                return Api.Fail(40300, "未识别租户");

            var aiEnabled = http.RequestServices.GetRequiredService<IConfiguration>().GetValue("Ai:Enabled", false);
            if (!aiEnabled)
                return Api.Fail(50300, "AI 助手未启用（Ai:Enabled=false）");

            Device? device = null;
            if (req.DeviceId != Guid.Empty)
                device = await db.Devices.FirstOrDefaultAsync(d => d.Id == req.DeviceId);

            var recent = await db.FaultEvents
                .Where(f => f.DeviceId == req.DeviceId)
                .OrderByDescending(f => f.CreatedAt)
                .Take(10).ToListAsync();

            // RAG：编码当前故障模式 → 检索同租户相似故障模式
            var vec = FaultEmbedding.Encode(req.FaultCode, req.Severity);
            var similar = await vectors.SearchSimilarAsync(current.TenantId, vec, 5);

            // 构造问诊上下文（仅含本租户数据，隔离由 EF 过滤器保证）
            var ctx = BuildContext(device, recent, similar);
            var response = await chat.GetResponseAsync(new List<ChatMessage>
            {
                new(ChatRole.System, "你是 PLC·AIOT 设备运维诊断助手，仅依据提供的租户内故障上下文给出中文诊断建议。"),
                new(ChatRole.User, ctx),
            });

            // 将本次故障模式写入向量库（best-effort）
            await vectors.UpsertPatternAsync(current.TenantId, req.FaultCode, req.Severity, vec);

            return Api.Ok(new DiagnoseResponse(
                response.Text,
                similar.Select(s => new SimilarPattern(s.Code, s.Severity, s.Score)).ToArray()));
        }))
        .RequireAuthorization("TenantUser");
    }

    private static string BuildContext(Device? device, IReadOnlyList<FaultEvent> recent, IReadOnlyList<FaultPatternHit> similar)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"设备：{(device is null ? "未知" : device.Name)}");
        sb.AppendLine("近期故障：");
        foreach (var f in recent) sb.AppendLine($" - {f.Code} ({f.Severity}) @ {f.CreatedAt:u}");
        sb.AppendLine("相似历史模式：");
        foreach (var s in similar) sb.AppendLine($" - {s.Code} ({s.Severity}) score={s.Score:F2}");
        return sb.ToString();
    }
}

public sealed record DiagnoseRequest(Guid DeviceId, string FaultCode, string Severity);
public sealed record DiagnoseResponse(string Diagnosis, IReadOnlyList<SimilarPattern> SimilarPatterns);
public sealed record SimilarPattern(string Code, string Severity, float Score);

// ===================== MCP Server 工具（tid 作用域隔离）=====================
// 外部 AI Agent 经 /mcp（Streamable HTTP + SSE）问诊；鉴权由全局 UseAuthentication 注入 JWT(tid)，
// UseTenantContext 解析 tid → 以下工具一律经 ICurrentTenant 强隔离，未鉴权/跨租户访问直接拒绝。
[McpServerToolType]
public sealed class PlcMcpTools
{
    private readonly IServiceScopeFactory _scopeFactory;
    public PlcMcpTools(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    [McpServerTool, Description("列出当前租户下的设备（含名称与在线状态）")]
    public async Task<string> ListDevicesAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var (_, db) = Resolve(scope);
        var devices = await db.Devices.Select(d => new { d.Id, d.Name, d.Status }).ToListAsync();
        return Json.Stringify(devices);
    }

    [McpServerTool, Description("查询当前租户的故障事件（可选按设备过滤）")]
    public async Task<string> GetFaultsAsync(string? deviceId = null)
    {
        using var scope = _scopeFactory.CreateScope();
        var (_, db) = Resolve(scope);
        var q = db.FaultEvents.AsQueryable();
        if (Guid.TryParse(deviceId, out var did) && did != Guid.Empty)
            q = q.Where(f => f.DeviceId == did);
        var faults = await q.OrderByDescending(f => f.CreatedAt).Take(50)
            .Select(f => new { f.Id, f.Code, f.Severity, f.Status, f.CreatedAt }).ToListAsync();
        return Json.Stringify(faults);
    }

    [McpServerTool, Description("获取指定设备的遥测摘要（故障计数等）")]
    public async Task<string> GetTelemetrySummaryAsync(string deviceId)
    {
        using var scope = _scopeFactory.CreateScope();
        var (_, db) = Resolve(scope);
        if (!Guid.TryParse(deviceId, out var did) || did == Guid.Empty)
            throw new InvalidOperationException("deviceId 非法");
        var count = await db.FaultEvents.CountAsync(f => f.DeviceId == did);
        var device = await db.Devices.FirstOrDefaultAsync(d => d.Id == did);
        return Json.Stringify(new { deviceId = did, deviceName = device?.Name, faultCount = count });
    }

    private static (ICurrentTenant, BaseDbContext) Resolve(IServiceScope scope)
    {
        var current = scope.ServiceProvider.GetRequiredService<ICurrentTenant>();
        if (current.TenantId == Guid.Empty)
            throw new InvalidOperationException("MCP 工具拒绝未鉴权/跨租户访问");
        var db = scope.ServiceProvider.GetRequiredService<BaseDbContext>();
        return (current, db);
    }
}

// 轻量 JSON 序列化辅助（避免与 System.Text.Json 命名冲突）
internal static class Json
{
    public static string Stringify<T>(T value) => System.Text.Json.JsonSerializer.Serialize(value);
}
