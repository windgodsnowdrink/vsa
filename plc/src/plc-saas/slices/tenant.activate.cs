// slices/tenant.activate.cs — 租户详情（超管，§5.1 GET /tenants/{id}）
#include "../infra.cs"

public static class TenantActivateApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/tenants/{id}", static async (Guid id, IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new GetTenantQuery(id)))))
            .RequireAuthorization("SuperAdminOnly");
    }
}

public sealed record TenantDetailResponse(
    Guid Id, string Name, string Slug, string Status, DateTime CreatedAt,
    int DeviceCount, int ActiveDevices, int OpenFaults);

public sealed record GetTenantQuery(Guid Id) : IRequest<TenantDetailResponse>;
public sealed class GetTenantHandler : IRequestHandler<GetTenantQuery, TenantDetailResponse>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    public GetTenantHandler(BaseDbContext db, ICurrentTenant current) => (_db, _current) = (db, current);

    public async ValueTask<TenantDetailResponse> Handle(GetTenantQuery q, CancellationToken ct)
    {
        _current.IsRoot = true; // 超管跨租户读
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Id == q.Id, ct)
            ?? throw new DomainException(40400, "租户不存在");
        var devices = await _db.Devices.IgnoreQueryFilters().Where(d => d.TenantId == tenant.Id).ToListAsync(ct);
        var openFaults = await _db.FaultEvents.IgnoreQueryFilters()
            .CountAsync(f => f.TenantId == tenant.Id && f.Status == FaultStatus.open, ct);
        return new TenantDetailResponse(
            tenant.Id, tenant.Name, tenant.Slug, tenant.Status.ToString(), tenant.CreatedAt,
            devices.Count, devices.Count(d => d.Status == DeviceStatus.online), openFaults);
    }
}
