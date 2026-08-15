// slices/faults.ack.cs — 确认告警（既有 PLC 域，§5.4 / AC-04）
// POST /faults/{id}/ack（租户用户，非分析师）。分析师被 NotAnalyst 策略拒绝 → 403。
// 跨租户 fault 被全局过滤器隐藏 → 404（不泄露归属，AC-03）。成功后经 SignalR 直推（ADR-105）。
#include "../infra.cs"

public static class FaultsAckApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapPost("/faults/{id}/ack", static async (Guid id, AckRequest? req, HttpContext ctx, IMediator m) =>
            await EndpointHelpers.Run(async () =>
                Api.Ok(await m.Send(new AckFaultCommand(id, req?.Note, ctx.User.CurrentUserId())))))
            .RequireAuthorization("TenantUser", "NotAnalyst");
    }
}

public sealed record AckRequest(string? Note);
public sealed record AckFaultResponse(Guid Id, string Status, Guid? AckedBy, DateTime? AckedAt);

public sealed record AckFaultCommand(Guid Id, string? Note, Guid ActorId) : IRequest<AckFaultResponse>;
public sealed class AckFaultHandler : IRequestHandler<AckFaultCommand, AckFaultResponse>
{
    private readonly BaseDbContext _db;
    private readonly ICurrentTenant _current;
    private readonly IMediator _mediator;
    public AckFaultHandler(BaseDbContext db, ICurrentTenant current, IMediator mediator)
        => (_db, _current, _mediator) = (db, current, mediator);

    public async ValueTask<AckFaultResponse> Handle(AckFaultCommand cmd, CancellationToken ct)
    {
        var fault = await _db.FaultEvents.FirstOrDefaultAsync(f => f.Id == cmd.Id, ct)
            ?? throw new DomainException(40400, "告警不存在"); // 跨租户不可见 → 404，不泄露

        if (fault.Status != FaultStatus.acked)
        {
            fault.Status = FaultStatus.acked;
            fault.AckedBy = cmd.ActorId;
            fault.AckedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            // 热路径强一致：域内事件经 Mediator 通知 → SignalR 直推（ADR-105）
            await _mediator.Publish(new FaultAcked(_current.TenantId, fault.Id, cmd.ActorId, fault.AckedAt.Value), ct);
        }

        return new AckFaultResponse(fault.Id, fault.Status.ToString(), fault.AckedBy, fault.AckedAt);
    }
}
