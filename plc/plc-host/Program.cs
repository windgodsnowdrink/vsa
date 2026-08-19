using Carter;
using Plc.Host.Foundation;
using Plc.Host.Foundation.Resilience;
using Plc.Host.Modules;
using Plc.Host.Plugins;
using Plc.Plugins.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Tier1：插件体系——宿主仅持有 PluginManager，插件程序集经 ALC 运行时加载，绝不进依赖图
builder.Services.AddSingleton<PluginManager>();
builder.Services.AddHostedService<PluginHostService>();

// 弹性（Polly）：构建命名 ResiliencePipeline 目录，供各能力模块注入
builder.Services.AddSingleton<ResilienceCatalog>();

// Tier2：基础能力模块化（Scrutor 程序集扫描）+ Carter 模块化 HTTP 端点
builder.Services.AddCarter();
builder.Services.AddCapabilityModules();

var app = builder.Build();

app.MapGet("/", () => "PLC Plugin Host — Tier1:Plugins(ALC) + Tier2:Foundation Modules(Scrutor+Polly+Carter)");

// 模块化 HTTP 端点：Carter 自动发现全部 ICarterModule（含 PluginsModule / SystemInfoCapability）
app.MapCarter();

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
