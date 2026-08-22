using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Plc.Host.Foundation;
using ScottPlot;

namespace Plc.Host.Modules;

/// <summary>
/// ScottPlot 图表能力：自包含（无外部依赖），用于 HMI/看板生成遥测图表。
/// <para>
/// 宿主作为服务器，将图表以 <b>SVG 矢量</b> 经 HTTP 端点返回，HMI 前端直接 <c>&lt;img&gt;</c> 渲染。
/// 采用 SVG 而非 PNG：ScottPlot 的 PNG 栅格路径（SkiaSharp）在本宿主运行环境会挂死（实测 &gt;40s 无响应），
/// 而 SVG 为纯 C# 矢量生成、不经过栅格编码，稳定且对 Web/HMI 更友好（可缩放、体积小、无原生依赖卡顿）。
/// </para>
/// </summary>
public sealed class ScottPlotCapability : ICapabilityModule, ICarterModule
{
    public string Id => "chart.scottplot";

    public string Name => "ScottPlot 图表（HMI/看板，SVG 矢量）";

    public int Order => 20;

    public void RegisterServices(IServiceCollection services)
    {
        // ScottPlot 为纯库，无需 DI 注册；此处仅作为能力装配占位。
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/sys/chart/telemetry", () =>
        {
            var plot = new Plot();
            var rand = new Random(42);
            double[] data = Enumerable.Range(0, 120).Select(_ => rand.NextDouble() * 100).ToArray();
            plot.Add.Signal(data);
            plot.Title("设备遥测示例（随机采样）");
            plot.XLabel("采样点");
            plot.YLabel("值");

            // 生成含 SVG 的 HTML 页，提取 <svg> 矢量片段返回（避开挂死的 PNG 栅格路径）
            string html = plot.GetSvgHtml(800, 400);
            string svg = ExtractSvg(html);
            return Results.Content(svg, "image/svg+xml");
        });
    }

    private static string ExtractSvg(string html)
    {
        int start = html.IndexOf("<svg", StringComparison.OrdinalIgnoreCase);
        int end = html.LastIndexOf("</svg>", StringComparison.OrdinalIgnoreCase);
        if (start >= 0 && end > start)
        {
            return html.Substring(start, end + "</svg>".Length - start);
        }

        return html; // 兜底：原样返回
    }
}
