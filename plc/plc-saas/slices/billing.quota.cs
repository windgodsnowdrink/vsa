// slices/billing.quota.cs — 配额阈值与超限策略（Billing 上下文，§5.3 / G8）
// GET /billing/quota（租户用户）。返回阈值与动作（Warn/Throttle），无货币字段。
#include "../infra.cs"

public static class BillingQuotaApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/billing/quota", static async (IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new GetQuotaQuery()))))
            .RequireAuthorization("TenantUser");
    }
}

public sealed record GetQuotaQuery : IRequest<QuotaResponse>;
public sealed class GetQuotaHandler : IRequestHandler<GetQuotaQuery, QuotaResponse>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    public GetQuotaHandler(BaseDbContext db, ICurrentTenant current) => (_db, _current) = (db, current);

    public async ValueTask<QuotaResponse> Handle(GetQuotaQuery _, CancellationToken ct)
    {
        var sub = await _db.Subscriptions.FirstOrDefaultAsync(s => s.TenantId == _current.TenantId, ct)
            ?? throw new DomainException(40400, "租户无有效订阅");
        var policies = await _db.QuotaPolicies
            .Where(q => q.PlanId == sub.PlanId)
            .Select(q => new QuotaPolicyDto(q.Metric.ToString(), q.ThresholdPct, q.Action.ToString()))
            .ToListAsync(ct);
        return new QuotaResponse(sub.PlanId, policies);
    }
}
