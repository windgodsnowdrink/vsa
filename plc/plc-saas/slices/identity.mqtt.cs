// slices/identity.mqtt.cs — 派生短时 MQTT 凭据（Identity 上下文，§5.2 / ADR-107）
// POST /mqtt/credential（租户用户）。凭据内嵌 tid；EMQX 经 JWT auth + ACL 隔离（方案 B）。
#include "../infra.cs"

public static class IdentityMqttApi
{
    public static void Map(RouteGroupBuilder api)
    {
        api.MapPost("/mqtt/credential", static async (IMediator m) =>
            await EndpointHelpers.Run(async () => Api.Ok(await m.Send(new IssueMqttCredentialCommand()))))
            .RequireAuthorization("TenantUser");
    }
}

public sealed record MqttCredentialResponse(
    string Url, string Username, string Password, DateTime ExpiresAt, string TokenType);

public sealed record IssueMqttCredentialCommand : IRequest<MqttCredentialResponse>;
public sealed class IssueMqttCredentialHandler : IRequestHandler<IssueMqttCredentialCommand, MqttCredentialResponse>
{
    private readonly IConfiguration _jwt;
    private readonly IConfiguration _cfg;
    private readonly ICurrentTenant _current;
    private readonly IHttpContextAccessor _ctx;
    public IssueMqttCredentialHandler(IConfiguration jwt, IConfiguration cfg, ICurrentTenant current, IHttpContextAccessor ctx)
        => (_jwt, _cfg, _current, _ctx) = (jwt, cfg, current, ctx);

    public ValueTask<MqttCredentialResponse> Handle(IssueMqttCredentialCommand _, CancellationToken ct)
    {
        var tid = _current.IsRoot ? SaasTenant.Root : _current.TenantId.ToString();
        var uid = _ctx.HttpContext?.User.CurrentUserId() ?? Guid.Empty;
        var token = JwtIssuer.IssueMqttToken(tid, uid.ToString(), _jwt);
        var minutes = double.Parse(_jwt["MqttTokenMinutes"]!);
        return ValueTask.FromResult(new MqttCredentialResponse(
            Url: _cfg["Mqtt:BrokerUrl"] ?? "mqtts://broker.plc-aiot.example:8883",
            Username: tid,
            Password: token,
            ExpiresAt: DateTime.UtcNow.AddMinutes(minutes),
            TokenType: "MQTT-JWT"));
    }
}
