using System.Diagnostics;
using System.Text.Json;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Plc.Host.Foundation;

namespace Plc.Host.Modules;

/// <summary>
/// HybridCache 设备影子缓存能力：一等公民、net11 原生（Microsoft.Extensions.Caching.Hybrid 10.x）。
/// <para>
/// 注入具体公共类 <see cref="HybridCache"/> 缓存设备影子（device shadow）：读取统一走
/// <c>GetOrCreateAsync(key, factory, ...)</c>，命中直接返回、未命中才执行工厂（设备/DB 慢路径）并写回，
/// 命中时可大幅降低对设备/DB 的访问延迟。L1(本地内存)+L2(分布式) 两级缓存由 HybridCache 托管。
/// </para>
/// </summary>
public sealed class HybridCacheCapability : ICapabilityModule, ICarterModule
{
    public string Id => "cache.hybrid";

    public string Name => "HybridCache 设备影子缓存";

    public int Order => 10;

    public void RegisterServices(IServiceCollection services)
    {
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                // L1(本地内存) 60s，L2(分布式/Redis) 5min；超过即回源设备/DB
                LocalCacheExpiration = TimeSpan.FromSeconds(60),
                Expiration = TimeSpan.FromMinutes(5),
            };
        });
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // 设备影子：用 GetOrCreateAsync 统一读取——命中直接返回，未命中才执行工厂（设备/DB 慢路径）并写回。
        // 注：HybridCache 10.x 不再暴露 IHybridCache 接口，注入具体公共类 HybridCache；
        // 读取入口为 GetOrCreateAsync(key, factory, options, tags, ct)，无独立 GetAsync。
        app.MapGet("/sys/cache/shadow/{deviceId}", async (
            string deviceId, HybridCache cache, CancellationToken ct) =>
        {
            var key = $"shadow:{deviceId}";
            var sw = Stopwatch.StartNew();
            var shadow = await cache.GetOrCreateAsync<string>(key, async token =>
            {
                // 仅在缓存未命中时执行：模拟从设备/DB 拉取影子（慢路径）
                await Task.Delay(150, token);
                return JsonSerializer.Serialize(new
                {
                    deviceId,
                    online = true,
                    temperature = 36.5,
                    updatedAt = DateTime.UtcNow,
                });
            }, cancellationToken: ct);
            sw.Stop();
            return Results.Ok(new { deviceId, shadow, source = "cache-or-device", latencyMs = sw.ElapsedMilliseconds });
        });

        // 清除某设备影子缓存（便于演示“下一次回源”）
        app.MapDelete("/sys/cache/shadow/{deviceId}", async (
            string deviceId, HybridCache cache, CancellationToken ct) =>
        {
            await cache.RemoveAsync($"shadow:{deviceId}", ct);
            return Results.Ok(new { deviceId, removed = true });
        });
    }
}
