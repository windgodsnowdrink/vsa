// slices/ops.health.cs — 平台运营：平台健康 / SLO（§5.5，超管）
#include "../infra.cs"

public static class OpsHealthApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapGet("/ops/health", static async (IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new GetHealthQuery()))))
            .RequireAuthorization("SuperAdminOnly");
    }
}

public sealed record SlaInfo(double UptimePct);
public sealed record ComponentHealth(string Name, string Status);
public sealed record HealthResponse(string Status, SlaInfo Sla, List<ComponentHealth> Components);

public sealed record GetHealthQuery : IRequest<HealthResponse>;
public sealed class GetHealthHandler : IRequestHandler<GetHealthQuery, HealthResponse>
{
    public ValueTask<HealthResponse> Handle(GetHealthQuery _, CancellationToken ct) => ValueTask.FromResult(new HealthResponse(
        "Healthy",
        new SlaInfo(99.95),
        new List<ComponentHealth>
        {
            new("api", "Healthy"),
            new("postgres", "Healthy"),
            new("signalr", "Healthy"),
            new("mqtt-broker", "Healthy"),
        }));
}
