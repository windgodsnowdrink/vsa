using Carter;
using Foundatio.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plc.Host.Foundation;

namespace Plc.Host.Modules;

/// <summary>
/// Foundatio 能力：评估其与现有能力栈的重叠（缓存↔HybridCache、队列/消息↔System.Threading.Channels）。
/// <para>
/// 本模块用其 <see cref="ICacheClient"/>（内存实现）演示缓存，证明 API 可用；但**不推荐在进程内使用**——
/// 缓存已由一等公民 HybridCache 覆盖，队列已由 Channel 覆盖。Foundatio 仅建议在其“分布式后端”
/// （Redis/Elastics/队列）确有需求时，作为可选外接能力引入，而非替代已落地的基础能力。
/// </para>
/// </summary>
public sealed class FoundatioCapability : ICapabilityModule, ICarterModule
{
    public string Id => "foundatio";

    public string Name => "Foundatio（重叠评估：缓存/队列）";

    public int Order => 45;

    public void RegisterServices(IServiceCollection services)
    {
        // 用内存实现演示 API；真实场景可换 RedisCacheClient（需外接 Redis）。
        services.AddSingleton<ICacheClient>(_ => new InMemoryCacheClient());
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sys/foundatio/cache/{key}", async (
            string key, ICacheClient cache, CancellationToken ct) =>
        {
            var hit = await cache.GetAsync<string>(key);
            if (hit.HasValue)
            {
                return Results.Ok(new { key, value = hit.Value, source = "foundatio-cache" });
            }

            await cache.SetAsync(key, $"value-{key}-{DateTime.UtcNow:HH:mm:ss}", TimeSpan.FromMinutes(5));
            return Results.Ok(new { key, source = "generated" });
        });
    }
}
