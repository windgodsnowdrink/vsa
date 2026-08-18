using Plc.Host.Plugins;
using Plc.Plugins.Contracts;

var builder = WebApplication.CreateBuilder(args);

// 插件体系：宿主仅持有 PluginManager，插件程序集经 ALC 运行时加载，绝不进依赖图
builder.Services.AddSingleton<PluginManager>();
builder.Services.AddHostedService<PluginHostService>();

var app = builder.Build();

// 插件清单
app.MapGet("/plugins", (PluginManager pm) =>
    pm.Plugins.Select(p => new { p.Id, p.Name, p.Version }));

// 健康检查（供编排/探针使用）
app.MapGet("/healthz", (PluginManager pm) =>
    Results.Ok(new { status = "healthy", plugins = pm.Plugins.Count }));

app.MapGet("/", () => "PLC Plugin Host — DotNetCorePlugins-style ALC plugin system (priority 1)");

app.Run();

/// <summary>
/// 宿主级后台服务：启动时发现并加载/启动全部插件，停止时优雅关闭。
/// </summary>
internal sealed class PluginHostService : IHostedService
{
    private readonly PluginManager _pluginManager;

    public PluginHostService(PluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _pluginManager.DiscoverAndLoad();
        return _pluginManager.StartAllAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _pluginManager.StopAllAsync();
    }
}
