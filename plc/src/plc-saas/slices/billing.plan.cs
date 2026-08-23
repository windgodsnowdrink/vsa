// slices/billing.plan.cs — 套餐档查询与切换（Billing 上下文，§5.3 / ADR-108）
// GET/POST /billing/plan。套餐档仅作配额差异化，无价格字段。
#include "../infra.cs"

public static class BillingPlanApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/billing/plan", static async (IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new GetPlanQuery()))))
            .RequireAuthorization("TenantUser");

        api.MapPost("/billing/plan", static async (PlanChangeRequest req, IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new ChangePlanCommand(req.PlanId)), "套餐已切换")))
            .RequireAuthorization("TenantAdminOnly");
    }
}

public sealed record PlanChangeRequest(Guid PlanId);

public sealed record GetPlanQuery : IRequest<PlanResponse>;
public sealed class GetPlanHandler : IRequestHandler<GetPlanQuery, PlanResponse>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    public GetPlanHandler(BaseDbContext db, ICurrentTenant current) => (_db, _current) = (db, current);

    public async ValueTask<PlanResponse> Handle(GetPlanQuery _, CancellationToken ct)
    {
        var sub = await _db.Subscriptions.FirstOrDefaultAsync(s => s.TenantId == _current.TenantId, ct)
            ?? throw new DomainException(40400, "租户无有效订阅");
        var plan = await _db.Plans.FirstAsync(p => p.Id == sub.PlanId, ct);
        return new PlanResponse(plan.Id, plan.Name, plan.DeviceQuota, plan.TelemetryQuota);
    }
}

public sealed record ChangePlanCommand(Guid PlanId) : IRequest<PlanResponse>;
public sealed class ChangePlanHandler : IRequestHandler<ChangePlanCommand, PlanResponse>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    public ChangePlanHandler(BaseDbContext db, ICurrentTenant current) => (_db, _current) = (db, current);

    public async ValueTask<PlanResponse> Handle(ChangePlanCommand cmd, CancellationToken ct)
    {
        var plan = await _db.Plans.FirstOrDefaultAsync(p => p.Id == cmd.PlanId, ct)
            ?? throw new DomainException(40400, "套餐档不存在");
        var sub = await _db.Subscriptions.FirstOrDefaultAsync(s => s.TenantId == _current.TenantId, ct)
            ?? throw new DomainException(40400, "租户无有效订阅");
        sub.PlanId = plan.Id; // 仅变更配额，不计费（ADR-108）
        await _db.SaveChangesAsync(ct);
        return new PlanResponse(plan.Id, plan.Name, plan.DeviceQuota, plan.TelemetryQuota);
    }
}
