// slices/billing.usage.cs — 用量计量（Billing 上下文，§5.3 / ADR-108）
// GET /billing/usage（租户用户）。仅计量，无任何 price/currency/amount/money 字段。
#include "../infra.cs"

// —— 共享 DTO（Billing 三端点复用）——
public sealed record PlanResponse(Guid Id, string Name, int DeviceQuota, int TelemetryQuota);
public sealed record QuotaRemaining(long Devices, long Telemetry);
public sealed record UsageResponse(
    string Period, int DeviceCount, long TelemetryPoints, QuotaRemaining QuotaRemaining, PlanResponse Plan);
public sealed record QuotaPolicyDto(string Metric, decimal ThresholdPct, string Action);
public sealed record QuotaResponse(Guid PlanId, List<QuotaPolicyDto> Policies);

public static class BillingUsageApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/billing/usage", static async (IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new GetUsageQuery()))))
            .RequireAuthorization("TenantUser");
    }
}

public sealed record GetUsageQuery : IRequest<UsageResponse>;
public sealed class GetUsageHandler : IRequestHandler<GetUsageQuery, UsageResponse>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    public GetUsageHandler(BaseDbContext db, ICurrentTenant current) => (_db, _current) = (db, current);

    public async ValueTask<UsageResponse> Handle(GetUsageQuery _, CancellationToken ct)
    {
        var tenantId = _current.TenantId;
        var period = DateTime.UtcNow.ToString("yyyy-MM");

        var sub = await _db.Subscriptions.FirstOrDefaultAsync(s => s.TenantId == tenantId, ct)
            ?? throw new DomainException(40400, "租户无有效订阅");
        var plan = await _db.Plans.FirstAsync(p => p.Id == sub.PlanId, ct);

        var deviceCount = await _db.Devices.CountAsync(d => d.TenantId == tenantId, ct);
        var meter = await _db.UsageMeters.FirstOrDefaultAsync(m => m.TenantId == tenantId && m.Period == period, ct);
        var telemetry = meter?.TelemetryPoints ?? 0;

        return new UsageResponse(
            period, deviceCount, telemetry,
            new QuotaRemaining(plan.DeviceQuota - deviceCount, plan.TelemetryQuota - telemetry),
            new PlanResponse(plan.Id, plan.Name, plan.DeviceQuota, plan.TelemetryQuota));
    }
}
