using Microsoft.Extensions.Logging;

namespace Plc.Plugins.Contracts;

/// <summary>
/// 插件契约（宿主与插件共享程序集）。
/// 任一程序集只要实现 <see cref="IPlugin"/> 且无参构造，即可被宿主以 ALC 隔离加载。
/// </summary>
public interface IPlugin
{
    /// <summary>插件唯一标识，如 "demo.modbus"。</summary>
    string Id { get; }

    /// <summary>展示名。</summary>
    string Name { get; }

    /// <summary>语义化版本。</summary>
    string Version { get; }

    /// <summary>宿主启动时调用，用于注册设备协议 / 业务模块 / 后台任务等。</summary>
    Task StartAsync(IPluginContext context, CancellationToken cancellationToken = default);

    /// <summary>宿主停止或热卸载时调用，释放资源。</summary>
    Task StopAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 插件运行时上下文：宿主注入的服务、日志与所在目录。
/// 插件应通过 <see cref="Services"/> 解析宿主能力，而非直接依赖宿主内部类型。
/// </summary>
public interface IPluginContext
{
    /// <summary>宿主根服务容器（插件可解析宿主注册的公共服务）。</summary>
    IServiceProvider Services { get; }

    /// <summary>插件专属日志（实为宿主 ILogger，按插件名分类）。</summary>
    ILogger Logger { get; }

    /// <summary>插件程序集所在目录（用于读取配套配置 / 资源）。</summary>
    string PluginDirectory { get; }

    /// <summary>宿主停止信号，插件应据此优雅退出。</summary>
    CancellationToken AppStopping { get; }
}
