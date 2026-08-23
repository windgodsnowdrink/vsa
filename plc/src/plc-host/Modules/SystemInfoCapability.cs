using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plc.Host.Foundation;
using Plc.Host.Foundation.Resilience;

namespace Plc.Host.Modules;

/// <summary>
/// 系统信息与弹性演示能力：同时实现 <see cref="ICapabilityModule"/>（Scrutor 扫描装配）
/// 与 <see cref="ICarterModule"/>（Carter 模块化 HTTP 端点）。
/// 这是「基础能力模块化」的范式样本——后续其他能力（ScottPlot / HybridCache / OTel…）
/// 均照此结构新增一个类即可，无需改动宿主启动代码。
/// </summary>
public sealed class SystemInfoCapability : ICapabilityModule, ICarterModule
{
    // 演示用：模拟一个间歇性失败的外部调用，验证 Polly 重试/熔断。
    private static readonly ConcurrentQueue<DateTime> s_callLog = new();

    public string Id => "sys.info";

    public string Name => "系统信息与弹性演示";

    public int Order => 0;

    public void RegisterServices(IServiceCollection services)
    {
        // 该能力无额外服务；展示点：能力可在此注册自己的类型。
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // 宿主/运行时基础信息
        app.MapGet("/sys/info", () => new
        {
            host = "PLC Plugin Host",
            framework = $".NET {RuntimeInformation.FrameworkDescription}",
            os = RuntimeInformation.OSDescription.Trim(),
            processArch = RuntimeInformation.ProcessArchitecture.ToString(),
            tier1 = "Plugins (ALC, 第三方设备协议)",
            tier2 = "Capability Modules (Scrutor + Polly + Carter)",
            timestamp = DateTime.UtcNow,
        });

        // 列出经 Scrutor 发现的所有能力模块（自证模块化装配生效）
        app.MapGet("/sys/capabilities", (IEnumerable<ICapabilityModule> modules, ResilienceCatalog resilience) => new
        {
            modules = modules.Select(m => new { m.Id, m.Name, m.Order }),
            resiliencePipelines = resilience.Names,
        });

        // 弹性演示：用 Polly default 管线包裹一个间歇性失败的操作
        app.MapGet("/sys/resilience-demo", async (
            ResilienceCatalog catalog,
            CancellationToken ct) =>
        {
                var pipeline = catalog.Get("default");
                try
                {
                    string? result = null;
                    await pipeline.ExecuteAsync(async _ => { result = await FlakyFetchAsync(ct); });
                    return Results.Ok(new { ok = true, result, totalCalls = s_callLog.Count });
                }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: $"弹性策略耗尽仍未成功: {ex.GetType().Name}",
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        });
    }

    /// <summary>每 3 次成功 1 次，用于演示 Polly 重试把瞬时失败消化掉。</summary>
    private static async Task<string> FlakyFetchAsync(CancellationToken ct)
    {
        await Task.Delay(20, ct);
        var attempt = s_callLog.Count + 1;
        s_callLog.Enqueue(DateTime.UtcNow);
        if (attempt % 3 != 0)
        {
            throw new HttpRequestException($"模拟瞬时故障（第 {attempt} 次调用）");
        }

        return $"外部调用在第 {attempt} 次成功";
    }
}
