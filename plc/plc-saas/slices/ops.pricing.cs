// slices/ops.pricing.cs — 平台运营：计费策略 / 套餐档定义（§5.5，超管）
// 无价格字段（ADR-108）。
#include "../infra.cs"

public static class OpsPricingApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/ops/pricing", static async (IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new GetPricingQuery()))))
            .RequireAuthorization("SuperAdminOnly");
    }
}

public sealed record PricingResponse(List<PlanResponse> Plans, List<QuotaPolicyDto> QuotaPolicies);

public sealed record GetPricingQuery : IRequest<PricingResponse>;
public sealed class GetPricingHandler : IRequestHandler<GetPricingQuery, PricingResponse>
{
    private readonly BaseDbContext _db;
    public GetPricingHandler(BaseDbContext db) => _db = db;

    public async ValueTask<PricingResponse> Handle(GetPricingQuery _, CancellationToken ct)
    {
        var plans = await _db.Plans.IgnoreQueryFilters()
            .Select(p => new PlanResponse(p.Id, p.Name, p.DeviceQuota, p.TelemetryQuota)).ToListAsync(ct);
        var policies = await _db.QuotaPolicies
            .Select(q => new QuotaPolicyDto(q.Metric.ToString(), q.ThresholdPct, q.Action.ToString())).ToListAsync(ct);
        return new PricingResponse(plans, policies);
    }
}
