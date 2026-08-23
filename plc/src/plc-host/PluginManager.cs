using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Plc.Plugins.Contracts;

namespace Plc.Host.Plugins;

/// <summary>
/// 单条已加载插件：持有实例、隔离上下文与来源路径，负责卸载。
/// </summary>
public sealed class LoadedPlugin : IDisposable
{
    public LoadedPlugin(IPlugin instance, PluginLoadContext context, string sourcePath)
    {
        Instance = instance;
        Context = context;
        SourcePath = sourcePath;
    }

    public IPlugin Instance { get; }

    public PluginLoadContext Context { get; }

    public string SourcePath { get; }

    public void Dispose() => Context.Unload();
}

/// <summary>
/// 插件管理器：发现 plugins/ 目录下的插件程序集，以独立 ALC 隔离加载，
/// 支持启动/停止、热重载（文件变更后卸载重建）与整体卸载。
/// </summary>
public sealed class PluginManager : IDisposable
{
    private readonly ILogger<PluginManager> _logger;
    private readonly IServiceProvider _services;
    private readonly string _pluginsDir;
    private readonly CancellationToken _appStopping;
    private readonly List<LoadedPlugin> _loaded = new();
    private readonly object _gate = new();
    private FileSystemWatcher? _watcher;
    private bool _disposed;

    public PluginManager(
        ILogger<PluginManager> logger,
        IServiceProvider services,
        IHostApplicationLifetime lifetime,
        IConfiguration configuration)
    {
        _logger = logger;
        _services = services;
        _pluginsDir = configuration["Plugins:Directory"] ?? "plugins";
        _appStopping = lifetime.ApplicationStopping;
    }

    /// <summary>当前已加载插件（快照）。</summary>
    public IReadOnlyCollection<IPlugin> Plugins
    {
        get
        {
            lock (_gate)
            {
                return _loaded.Select(x => x.Instance).ToList();
            }
        }
    }

    /// <summary>扫描插件目录并逐个隔离加载（不启动）。</summary>
    public void DiscoverAndLoad()
    {
        if (!Directory.Exists(_pluginsDir))
        {
            _logger.LogWarning("插件目录不存在，跳过加载: {Dir}", _pluginsDir);
            return;
        }

        foreach (var dir in Directory.GetDirectories(_pluginsDir))
        {
            var dll = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories)
                .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f)
                    .Equals(Path.GetFileName(dir), StringComparison.OrdinalIgnoreCase));
            if (dll is null)
            {
                _logger.LogDebug("跳过（无同名程序集）: {Dir}", dir);
                continue;
            }

            try
            {
                LoadFrom(dll);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载插件失败: {Dll}", dll);
            }
        }

        StartWatching();
    }

    private void LoadFrom(string dll)
    {
        var directory = Path.GetDirectoryName(dll)!;
        var ctx = new PluginLoadContext(directory);
        var assembly = ctx.LoadFromAssemblyPath(Path.GetFullPath(dll));

        var pluginType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t)
                                 && !t.IsAbstract
                                 && t.GetConstructor(Type.EmptyTypes) is not null);
        if (pluginType is null)
        {
            _logger.LogWarning("未找到无参构造的 IPlugin 实现，跳过: {Dll}", dll);
            ctx.Unload();
            return;
        }

        var instance = (IPlugin)Activator.CreateInstance(pluginType)!;
        var lp = new LoadedPlugin(instance, ctx, dll);
        lock (_gate)
        {
            _loaded.Add(lp);
        }

        _logger.LogInformation(
            "已加载插件 {Name} v{Version} [{Id}] @ {Dll}",
            instance.Name, instance.Version, instance.Id, dll);
    }

    /// <summary>启动全部已加载插件。</summary>
    public async Task StartAllAsync()
    {
        foreach (var plugin in Plugins)
        {
            try
            {
                await plugin.StartAsync(MakeContext(plugin), _appStopping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "启动插件失败: {Id}", plugin.Id);
            }
        }
    }

    /// <summary>停止全部已加载插件。</summary>
    public async Task StopAllAsync()
    {
        foreach (var plugin in Plugins)
        {
            try
            {
                await plugin.StopAsync(_appStopping);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "停止插件失败: {Id}", plugin.Id);
            }
        }
    }

    private IPluginContext MakeContext(IPlugin plugin)
    {
        LoadedPlugin? lp;
        lock (_gate)
        {
            lp = _loaded.FirstOrDefault(x => x.Instance == plugin);
        }

        var dir = lp?.SourcePath is null ? _pluginsDir : Path.GetDirectoryName(lp.SourcePath)!;
        var logger = _services.GetRequiredService<ILoggerFactory>().CreateLogger("Plugin:" + plugin.Id);
        return new PluginContext(_services, logger, dir, _appStopping);
    }

    /// <summary>
    /// 热重载：监听插件目录，文件变更后整体卸载重建（演示级；
    /// 生产应做单插件粒度 reload + 防抖 + 兼容正在处理的请求）。
    /// </summary>
    private void StartWatching()
    {
        try
        {
            _watcher = new FileSystemWatcher(_pluginsDir)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                Filter = "*.dll",
            };
            _watcher.Changed += (_, _) => _ = ReloadAllAsync();
            _watcher.Created += (_, _) => _ = ReloadAllAsync();
            _watcher.Deleted += (_, _) => _ = ReloadAllAsync();
            _watcher.EnableRaisingEvents = true;
            _logger.LogInformation("已启用插件热重载监听: {Dir}", _pluginsDir);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "插件热重载监听启动失败（不影响静态加载）");
        }
    }

    private async Task ReloadAllAsync()
    {
        if (_disposed)
        {
            return;
        }

        _logger.LogInformation("检测到插件变更，执行热重载…");
        await StopAllAsync();
        List<LoadedPlugin> snapshot;
        lock (_gate)
        {
            snapshot = _loaded.ToList();
            _loaded.Clear();
        }

        foreach (var lp in snapshot)
        {
            lp.Dispose();
        }

        DiscoverAndLoad();
        await StartAllAsync();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _watcher?.Dispose();
        List<LoadedPlugin> snapshot;
        lock (_gate)
        {
            snapshot = _loaded.ToList();
            _loaded.Clear();
        }

        foreach (var lp in snapshot)
        {
            lp.Dispose();
        }
    }
}
