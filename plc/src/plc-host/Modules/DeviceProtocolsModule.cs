using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Plc.Host.Devices;
using Plc.Host.Foundation;
using Plc.Plugins.Contracts;

namespace Plc.Host.Modules;

/// <summary>
/// 设备协议模块：暴露 Tier1 插件注册中心的发现与网关式读 / 写端点。
/// HMI / AIOT 经此发现可用协议（<c>/api/devices/protocols</c>）并透传访问现场设备，
/// 无需直接耦合任一具体协议驱动。
/// <para>
/// ADR-119 M4：读 / 写 / 批量读统一经 <see cref="DeviceSessionPool"/>（会话池 + 弹性 + 统一帧），
/// 不再「每次请求新建会话」。
/// </para>
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

        services.AddSingleton(sp => new DevicePoolOptions(
            Enabled: sp.GetRequiredService<IConfiguration>().GetValue<bool?>("DevicePool:Enabled") ?? true,
            MaxIdleSeconds: sp.GetRequiredService<IConfiguration>().GetValue<int?>("DevicePool:MaxIdleSeconds") ?? 600,
            ResiliencePipeline: sp.GetRequiredService<IConfiguration>()["DevicePool:ResiliencePipeline"] ?? "default"));

        services.AddSingleton(sp => new DeviceSessionPool(
            sp.GetRequiredService<IDeviceCatalog>(),
            sp.GetRequiredService<Concurrency.KeyedAsyncGate>(),
            sp.GetRequiredService<Foundation.Resilience.ResilienceCatalog>(),
            sp.GetRequiredService<DevicePoolOptions>(),
            sp.GetRequiredService<ILogger<DeviceSessionPool>>(),
            sp.GetService<Pipeline.AcquisitionPipeline>()));
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
            string protocolId,
            DeviceReadRequest body,
            IDeviceCatalog catalog,
            DeviceSessionPool pool,
            CancellationToken ct) =>
        {
            if (catalog.Get(protocolId) is null)
            {
                return Results.NotFound(new { protocolId, error = "未注册的设备协议" });
            }

            var options = new DeviceConnectionOptions(body.Host, body.Port, body.UnitId, body.TimeoutMs);
            var frame = await pool.ReadAsync(protocolId, options, body.ToReadRequest(), ct);
            return ToResult(protocolId, frame);
        });

        /// <summary>
        /// 批量读：一次请求聚合同一设备的多个地址。协议未覆写批量读时，契约的 DIM 默认实现
        /// 会串行转调单次读，因此任何协议都可用；协议覆写后（如 Modbus 合并连续地址）才有吞吐收益。
        /// </summary>
        app.MapPost("/api/devices/{protocolId}/readBatch", async (
            string protocolId,
            DeviceBatchReadRequest body,
            IDeviceCatalog catalog,
            DeviceSessionPool pool,
            CancellationToken ct) =>
        {
            if (catalog.Get(protocolId) is null)
            {
                return Results.NotFound(new { protocolId, error = "未注册的设备协议" });
            }

            if (body.Requests is null || body.Requests.Length == 0)
            {
                return Results.BadRequest(new { error = "requests 不能为空" });
            }

            var options = new DeviceConnectionOptions(body.Host, body.Port, body.UnitId, body.TimeoutMs);
            var requests = body.Requests.Select(r => r.ToReadRequest()).ToArray();
            var frames = await pool.ReadBatchAsync(protocolId, options, requests, body.DeviceId, ct);

            return Results.Ok(new
            {
                protocolId,
                requested = requests.Length,
                succeeded = frames.Count(f => f.IsSuccess),
                failed = frames.Count(f => !f.IsSuccess),
                frames = frames.Select(ToFrameDto).ToArray(),
            });
        });

        app.MapPost("/api/devices/{protocolId}/write", async (
            string protocolId,
            DeviceWriteRequest body,
            IDeviceCatalog catalog,
            DeviceSessionPool pool,
            CancellationToken ct) =>
        {
            if (catalog.Get(protocolId) is null)
            {
                return Results.NotFound(new { protocolId, error = "未注册的设备协议" });
            }

            var options = new DeviceConnectionOptions(body.Host, body.Port, body.UnitId, body.TimeoutMs);
            var frame = await pool.WriteAsync(protocolId, options, body.ToWriteRequest(), ct);
            return ToResult(protocolId, frame);
        });

        /// <summary>会话池状态（命中 / 驱逐 / 失败 / 投递）。</summary>
        app.MapGet("/api/devices/pool", (DeviceSessionPool pool) => Results.Ok(pool.Snapshot()));
    }

    private static IResult ToResult(string protocolId, DeviceFrame frame) =>
        frame.IsSuccess
            ? Results.Ok(new
            {
                protocolId,
                success = true,
                data = Convert.ToBase64String(frame.ToByteArray()),
                bytes = frame.Length,
                elapsedMs = Math.Round(frame.Elapsed.TotalMilliseconds, 3),
            })
            : Results.Ok(new
            {
                protocolId,
                success = false,
                error = frame.Error,
                elapsedMs = Math.Round(frame.Elapsed.TotalMilliseconds, 3),
            });

    private static object ToFrameDto(DeviceFrame frame) => frame.IsSuccess
        ? new
        {
            success = true,
            sequence = frame.Sequence,
            registerType = frame.RegisterType.ToString(),
            startAddress = frame.StartAddress,
            count = frame.Count,
            addressText = frame.AddressText,
            data = Convert.ToBase64String(frame.ToByteArray()),
            bytes = frame.Length,
            elapsedMs = Math.Round(frame.Elapsed.TotalMilliseconds, 3),
        }
        : new
        {
            success = false,
            sequence = frame.Sequence,
            registerType = frame.RegisterType.ToString(),
            startAddress = frame.StartAddress,
            error = frame.Error,
            elapsedMs = Math.Round(frame.Elapsed.TotalMilliseconds, 3),
        };
}

/// <summary>设备读请求（含连接参数）。</summary>
public sealed record DeviceReadRequest(
    string Host,
    int Port,
    byte UnitId,
    ushort StartAddress,
    ushort Count,
    RegisterType Type,
    int TimeoutMs = 3000)
{
    public ReadRequest ToReadRequest() =>
        new(StartAddress, Count, Type, null, ElementType.Unspecified);
}

/// <summary>批量读中的单个地址项。</summary>
public sealed record DeviceReadItem(
    ushort StartAddress,
    ushort Count,
    RegisterType Type,
    string? AddressText = null)
{
    public ReadRequest ToReadRequest() =>
        new(StartAddress, Count, Type, AddressText, ElementType.Unspecified);
}

/// <summary>设备批量读请求。</summary>
public sealed record DeviceBatchReadRequest(
    string Host,
    int Port,
    byte UnitId,
    DeviceReadItem[] Requests,
    int TimeoutMs = 3000,
    string? DeviceId = null);

/// <summary>设备写请求（含连接参数）。</summary>
public sealed record DeviceWriteRequest(
    string Host,
    int Port,
    byte UnitId,
    ushort StartAddress,
    RegisterType Type,
    ushort[] Values,
    int TimeoutMs = 3000)
{
    public WriteRequest ToWriteRequest() =>
        new(StartAddress, Type, Values, null, ElementType.Unspecified);
}
