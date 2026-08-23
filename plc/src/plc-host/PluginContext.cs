using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace Plc.Host.Plugins;

/// <summary>
/// 插件运行时上下文实现（宿主内部使用）。
/// </summary>
internal sealed class PluginContext : IPluginContext
{
    public PluginContext(
        IServiceProvider services,
        ILogger logger,
        string pluginDirectory,
        CancellationToken appStopping)
    {
        Services = services;
        Logger = logger;
        PluginDirectory = pluginDirectory;
        AppStopping = appStopping;
    }

    public IServiceProvider Services { get; }

    public ILogger Logger { get; }

    public string PluginDirectory { get; }

    public CancellationToken AppStopping { get; }
}
