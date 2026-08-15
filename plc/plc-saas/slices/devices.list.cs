// slices/devices.list.cs — 设备列表（既有 PLC 域，§5.4）
// GET /devices（租户用户）。全局过滤器自动附加 WHERE TenantId=当前（AC-02）。
#include "../infra.cs"

public static class DevicesApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/devices", static async (IMediator m, int page = 1, int limit = 20) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new ListDevicesQuery(page, limit)))))
            .RequireAuthorization("TenantUser");
    }
}

public sealed record DeviceResponse(Guid Id, string Name, string Status, DateTime? LastHeartbeat);

public sealed record ListDevicesQuery(int Page, int Limit) : IRequest<List<DeviceResponse>>;
public sealed class ListDevicesHandler : IRequestHandler<ListDevicesQuery, List<DeviceResponse>>
{
    private readonly BaseDbContext _db;
    public ListDevicesHandler(BaseDbContext db) => _db = db;

    public async ValueTask<List<DeviceResponse>> Handle(ListDevicesQuery q, CancellationToken ct)
    {
        var items = await _db.Devices
            .OrderBy(d => d.CreatedAt)
            .Skip((q.Page - 1) * q.Limit)
            .Take(q.Limit)
            .ToListAsync(ct);
        return items.Select(d => new DeviceResponse(d.Id, d.Name, d.Status.ToString(), d.LastHeartbeat)).ToList();
    }
}
