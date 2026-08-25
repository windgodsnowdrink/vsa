using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plc.Host.Foundation;
using Plc.Host.Gateway;

namespace Plc.Host.Modules;

/// <summary>
/// 边缘网关 RPC 代理：把 plc-gateway（SNET 协议 + Disruptor 消息记忆）的能力经 HTTP 暴露给
/// HMI / AIOT 上层。与 <see cref="DeviceProtocolsModule"/>（宿主自有 Tier1 协议目录）互补：
/// <c>/api/devices/*</c> 走宿主内置目录，<c>/api/gateway/*</c> 走边缘网关。
/// <para>
/// 网关未运行时，端点返回 503（<see cref="GatewayUnavailableException"/>）；网关 RPC 的业务异常
/// （如设备未找到）返回 502，便于上层区分「链路不通」与「业务逻辑错误」。
/// </para>
/// </summary>
public sealed class GatewayModule : ICapabilityModule, ICarterModule
{
    public string Id => "gateway.proxy";

    public string Name => "边缘网关 RPC 代理（SNET + Disruptor 消息记忆）";

    public int Order => 6;

    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<GatewayOptions>();
        services.AddSingleton<GatewayRpcClient>();
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/gateway/health", (GatewayRpcClient client) =>
            Results.Ok(new { connected = client.IsConnected, pipe = "plc-gateway" }));

        app.MapGet("/api/gateway/protocols", async (GatewayRpcClient client, CancellationToken ct) =>
        {
            try
            {
                return Results.Ok(await client.ListProtocolsAsync(ct));
            }
            catch (GatewayUnavailableException ex)
            {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        });

        app.MapGet("/api/gateway/devices", async (GatewayRpcClient client, CancellationToken ct) =>
        {
            try
            {
                return Results.Ok(await client.ListDevicesAsync(ct));
            }
            catch (GatewayUnavailableException ex)
            {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        });

        app.MapPost("/api/gateway/read", async (
            GatewayReadRequest body, GatewayRpcClient client, CancellationToken ct) =>
        {
            try
            {
                var data = await client.ReadAsync(body.DeviceId, body.Start, body.Count, body.Type, ct);
                return Results.Ok(new
                {
                    deviceId = body.DeviceId,
                    data = Convert.ToBase64String(data),
                    bytes = data.Length,
                });
            }
            catch (GatewayUnavailableException ex)
            {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                // 网关 RPC 业务异常（设备未找到 / 协议未注册 / 会话超时等）
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status502BadGateway);
            }
        });

        app.MapPost("/api/gateway/write", async (
            GatewayWriteRequest body, GatewayRpcClient client, CancellationToken ct) =>
        {
            try
            {
                await client.WriteAsync(body.DeviceId, body.Start, body.Type, body.Values, ct);
                return Results.Ok(new { deviceId = body.DeviceId, written = body.Values.Length });
            }
            catch (GatewayUnavailableException ex)
            {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: StatusCodes.Status502BadGateway);
            }
        });
    }
}

/// <summary>网关按需读请求体。</summary>
public sealed record GatewayReadRequest(string DeviceId, ushort Start, ushort Count, int Type);

/// <summary>网关按需写请求体。</summary>
public sealed record GatewayWriteRequest(string DeviceId, ushort Start, int Type, ushort[] Values);
