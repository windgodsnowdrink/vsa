using Carter;
using Microsoft.AspNetCore.Builder;
using Plc.Host.Plugins;

namespace Plc.Host.Modules;

/// <summary>
/// 插件管控端点（Carter 模块化）：将原先内联在 Program.cs 的 /plugins、/healthz 收口到模块，
/// 演示「宿主内置 HTTP 端点也走模块化（Carter）」而非散落的最小 API。
/// </summary>
public sealed class PluginsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/plugins", (PluginManager pm) =>
            pm.Plugins.Select(p => new { p.Id, p.Name, p.Version }));

        app.MapGet("/healthz", (PluginManager pm) =>
            Results.Ok(new { status = "healthy", plugins = pm.Plugins.Count }));
    }
}
