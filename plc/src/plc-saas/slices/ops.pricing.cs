// slices/ops.pricing.cs — 平台运营：计费策略 / 套餐档定义（§5.5，超管）
// 无价格字段（ADR-108）。

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
        // 先物化再在内存中投影：EF Core 无法把 enum.ToString() 翻译进 SQL 构造器（报 QuotaMetric→String 无强制转换）。
        var policies = (await _db.QuotaPolicies.ToListAsync(ct))
            .Select(q => new QuotaPolicyDto(q.Metric.ToString(), q.ThresholdPct, q.Action.ToString())).ToList();
        return new PricingResponse(plans, policies);
    }
}
