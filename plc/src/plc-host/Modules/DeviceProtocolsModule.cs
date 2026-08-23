using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plc.Host.Devices;
using Plc.Host.Foundation;
using Plc.Plugins.Contracts;

namespace Plc.Host.Modules;

/// <summary>
/// 设备协议模块：暴露 Tier1 插件注册中心的发现与网关式读 / 写端点。
/// HMI / AIOT 经此发现可用协议（<c>/api/devices/protocols</c>）并透传访问现场设备，
/// 无需直接耦合任一具体协议驱动。
/// </summary>
public sealed class DeviceProtocolsModule : ICapabilityModule, ICarterModule
{
    public string Id => "devices.protocols";

    public string Name => "设备协议目录（Tier1 插件注册中心）";

    public int Order => 5;

    public void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton<DeviceCatalog>();
        services.AddSingleton<IDeviceCatalog>(sp => sp.GetRequiredService<DeviceCatalog>());
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/devices/protocols", (IDeviceCatalog catalog) =>
            Results.Ok(catalog.All.Select(p => new
            {
                p.ProtocolId,
                p.DisplayName,
                p.Capabilities,
            })));

        app.MapPost("/api/devices/{protocolId}/read", async (
            string protocolId, DeviceReadRequest body, IDeviceCatalog catalog, CancellationToken ct) =>
        {
            var proto = catalog.Get(protocolId);
            if (proto is null)
            {
                return Results.NotFound(new { protocolId, error = "未注册的设备协议" });
            }

            var options = new DeviceConnectionOptions(body.Host, body.Port, body.UnitId, body.TimeoutMs);
            await using var session = proto.CreateSession(options);
            var data = await session.ReadAsync(new ReadRequest(body.StartAddress, body.Count, body.Type), ct);
            return Results.Ok(new
            {
                protocolId,
                data = Convert.ToBase64String(data),
                bytes = data.Length,
            });
        });

        app.MapPost("/api/devices/{protocolId}/write", async (
            string protocolId, DeviceWriteRequest body, IDeviceCatalog catalog, CancellationToken ct) =>
        {
            var proto = catalog.Get(protocolId);
            if (proto is null)
            {
                return Results.NotFound(new { protocolId, error = "未注册的设备协议" });
            }

            var options = new DeviceConnectionOptions(body.Host, body.Port, body.UnitId, body.TimeoutMs);
            await using var session = proto.CreateSession(options);
            await session.WriteAsync(new WriteRequest(body.StartAddress, body.Type, body.Values), ct);
            return Results.Ok(new { protocolId, written = body.Values.Length });
        });
    }
}

/// <summary>设备读请求（含连接参数）。</summary>
public sealed record DeviceReadRequest(
    string Host,
    int Port,
    byte UnitId,
    ushort StartAddress,
    ushort Count,
    RegisterType Type,
    int TimeoutMs = 3000);

/// <summary>设备写请求（含连接参数）。</summary>
public sealed record DeviceWriteRequest(
    string Host,
    int Port,
    byte UnitId,
    ushort StartAddress,
    RegisterType Type,
    ushort[] Values,
    int TimeoutMs = 3000);
