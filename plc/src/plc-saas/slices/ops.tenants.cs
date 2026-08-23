// slices/ops.tenants.cs — 平台运营：全部租户 + 健康（§5.5，超管）
#include "../infra.cs"

public static class OpsTenantsApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/ops/tenants", static async (IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new ListOpsTenantsQuery()))))
            .RequireAuthorization("SuperAdminOnly");
    }
}

public sealed record OpsTenantResponse(
    Guid Id, string Name, string Slug, string Status, DateTime CreatedAt,
    int DeviceCount, int OpenFaults, string PlanName);

public sealed record ListOpsTenantsQuery : IRequest<List<OpsTenantResponse>>;
public sealed class ListOpsTenantsHandler : IRequestHandler<ListOpsTenantsQuery, List<OpsTenantResponse>>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    public ListOpsTenantsHandler(BaseDbContext db, ICurrentTenant current) => (_db, _current) = (db, current);

    public async ValueTask<List<OpsTenantResponse>> Handle(ListOpsTenantsQuery _, CancellationToken ct)
    {
        _current.IsRoot = true; // 超管跨租户读（IgnoreQueryFilters）
        var tenants = await _db.Tenants.IgnoreQueryFilters().ToListAsync(ct);
        var result = new List<OpsTenantResponse>();
        foreach (var t in tenants)
        {
            var deviceCount = await _db.Devices.IgnoreQueryFilters().CountAsync(d => d.TenantId == t.Id, ct);
            var openFaults = await _db.FaultEvents.IgnoreQueryFilters()
                .CountAsync(f => f.TenantId == t.Id && f.Status == FaultStatus.open, ct);
            var planName = (await _db.Subscriptions.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.TenantId == t.Id, ct)) is { } sub
                ? (await _db.Plans.FirstAsync(p => p.Id == sub.PlanId, ct)).Name : "";
            result.Add(new OpsTenantResponse(t.Id, t.Name, t.Slug, t.Status.ToString(), t.CreatedAt, deviceCount, openFaults, planName));
        }
        return result;
    }
}
