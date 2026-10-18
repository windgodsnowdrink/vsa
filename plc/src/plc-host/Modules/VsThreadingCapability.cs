using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Threading;
using Plc.Host.Concurrency;
using Plc.Host.Foundation;

namespace Plc.Host.Modules;

/// <summary>
/// 并发底座（ADR-119 M2）：把 Microsoft.VisualStudio.Threading 装配进宿主 DI，
/// 为 M3 的 Disruptor 采集管道与 M4 的会话池提供统一的并发原语。
/// <para>提供三件东西：</para>
/// <list type="number">
/// <item><see cref="JoinableTaskContext"/> / <see cref="JoinableTaskFactory"/>：异步工作的跟踪与排空（优雅关闭用）。</item>
/// <item><see cref="KeyedAsyncGate"/>：按设备限流的异步闸门（<c>AsyncSemaphore</c>）——现场设备超并发表现为「不回包→超时」而非报错，必须本侧限流。</item>
/// <item><see cref="AsyncLazyRegistry{TKey, TValue}"/>：惰性异步单例注册表（<c>AsyncLazy&lt;T&gt;</c>）——M4 会话池据此保证「同一设备只创建一个会话」。</item>
/// </list>
/// <para>版本与 plc-gateway 钉齐（17.11.20），不浮动。</para>
/// </summary>
public sealed class VsThreadingCapability : ICapabilityModule, ICarterModule
{
    /// <summary>配置项：单设备并发上限。</summary>
    public const string PerDeviceLimitKey = "Concurrency:PerDeviceLimit";

    /// <summary>默认单设备并发上限：多数现场网关的舒适区是 2~8。</summary>
    public const int DefaultPerDeviceLimit = 4;

    public string Id => "concurrency.vsthreading";

    public string Name => "并发底座（Microsoft.VisualStudio.Threading）";

    // 早于设备协议模块（Order=5），保证数据面可用时并发原语已就绪
    public int Order => 2;

    public void RegisterServices(IServiceCollection services)
    {
        // JoinableTaskContext：无 UI 线程场景下也可构造（ASP.NET Core 无 SynchronizationContext），
        // 其 Factory 用于让 AsyncLazy 等原语的异步等待可被跟踪、关闭时可排空。
        services.AddSingleton(_ => new JoinableTaskContext());
        services.AddSingleton(sp => sp.GetRequiredService<JoinableTaskContext>().Factory);
        services.AddSingleton(sp => sp.GetRequiredService<JoinableTaskContext>().CreateCollection());

        services.AddSingleton(sp =>
        {
            var limit = sp.GetRequiredService<IConfiguration>().GetValue<int?>(PerDeviceLimitKey);
            return new KeyedAsyncGate(limit is > 0 ? limit.Value : DefaultPerDeviceLimit);
        });

        // 设备会话注册表：value 为 IDeviceSession（契约层类型），生命周期由注册表持有。
        services.AddSingleton(sp => new AsyncLazyRegistry<string, Plc.Plugins.Contracts.IDeviceSession>(
            _ => throw new InvalidOperationException(
                "会话工厂未注入：请在设备协议模块中提供按设备标识创建 IDeviceSession 的工厂（ADR-119 M4）"),
            sp.GetRequiredService<JoinableTaskFactory>()));
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sys/concurrency", (KeyedAsyncGate gate, AsyncLazyRegistry<string, Plc.Plugins.Contracts.IDeviceSession> registry) =>
            Results.Ok(new
            {
                perDeviceLimit = gate.PerKeyLimit,
                activeKeys = gate.ActiveKeyCount,
                totalInFlight = gate.TotalInFlight,
                inFlight = gate.Snapshot(),
                registeredSessions = registry.Count,
            }));

        /// <summary>
        /// 并发探针：用 <paramref name="parallel"/> 个并发任务争抢同一个 key，
        /// 统计实测峰值并发，用于验证闸门真的在限流（M6 门禁用）。
        /// </summary>
        app.MapPost("/sys/concurrency/probe", async (
            KeyedAsyncGate gate,
            string key,
            int parallel,
            int holdMs,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Results.BadRequest(new { error = "key 不能为空" });
            }

            parallel = Math.Clamp(parallel, 1, 256);
            holdMs = Math.Clamp(holdMs, 0, 5000);

            var peak = 0;
            var current = 0;
            async Task One()
            {
                using var lease = await gate.AcquireAsync(key, ct);
                var now = Interlocked.Increment(ref current);
                InterlockedMax(ref peak, now);
                try
                {
                    if (holdMs > 0)
                    {
                        await Task.Delay(holdMs, ct);
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref current);
                }
            }

            await Task.WhenAll(Enumerable.Range(0, parallel).Select(_ => One()));

            return Results.Ok(new
            {
                key,
                requested = parallel,
                perDeviceLimit = gate.PerKeyLimit,
                observedPeak = peak,
                throttled = peak <= gate.PerKeyLimit,
            });
        });
    }

    private static void InterlockedMax(ref int target, int value)
    {
        int seen;
        while (value > (seen = Volatile.Read(ref target)))
        {
            if (Interlocked.CompareExchange(ref target, value, seen) == seen)
            {
                return;
            }
        }
    }
}
