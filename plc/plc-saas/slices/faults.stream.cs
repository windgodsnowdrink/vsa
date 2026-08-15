// slices/faults.stream.cs — 实时告警流（SignalR Hub，§5.4 / ADR-105）
// GET /faults/stream（信息端点） + Hub "faults"（/hubs/faults/negotiate）。
// 告警确认等事件由 Hub 直推前端角标（AC-08）。
#include "../infra.cs"

public static class FaultsStreamApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/faults/stream", static () =>
            Api.Ok("SignalR 协商端点：/hubs/faults/negotiate（Hub 名 faults）")).AllowAnonymous();
    }
}

public sealed class FaultsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var tid = Context.User?.CurrentTenantId();
        if (!string.IsNullOrEmpty(tid) && tid != SaasTenant.Root)
            await Groups.AddToGroupAsync(Context.ConnectionId, tid);
        await base.OnConnectedAsync();
    }
}

// 域内事件 → SignalR 直推（同 UoW 强一致，ADR-105）
public sealed class FaultRealtimePushHandler : INotificationHandler<FaultAcked>
{
    private readonly IHubContext<FaultsHub> _hub;
    public FaultRealtimePushHandler(IHubContext<FaultsHub> hub) => _hub = hub;

    public async ValueTask Handle(FaultAcked n, CancellationToken ct)
    {
        var tid = n.TenantId == Guid.Empty ? SaasTenant.Root : n.TenantId.ToString();
        await _hub.Clients.Group(tid).SendAsync("badge.update",
            new { type = "fault.acked", tenantId = tid, faultId = n.FaultId, ackedBy = n.AckedBy }, ct);
    }
}
