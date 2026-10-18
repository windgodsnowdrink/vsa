// ────────────────────────────────────────────────────────────────────────────
// Slice: Plugins（IDaq/IMq + PluginLoaderHost + PluginLoadContext + PluginLease）
// 对应 8 项需求 §3（Daq 插件化采集）+ §5（DotNetCorePlugins ALC 插件）
// 宿主与插件通过 PlcVsa.Contracts.Plugins.IPlugin / IPluginContext / IPluginLogger 通信。
// ────────────────────────────────────────────────────────────────────────────
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MVT = Microsoft.VisualStudio.Threading;
using PlcVsa.Contracts.Devices;
using PlcVsa.Contracts.Plugins;
using PlcVsa.Server.Infrastructure;

namespace PlcVsa.Server.Slices;

/// <summary>§3 数采插件契约（独立于 IPlugin，按 shunnet/Daq 约定）。</summary>
public interface IDaq : IAsyncDisposable
{
    string Id { get; }
    string Name { get; }
    ValueTask OnStartAsync(IPluginContext ctx, CancellationToken ct);
    ValueTask OnStopAsync(CancellationToken ct);
    string GetStatus();
}

/// <summary>§3 MQ 插件契约（独立于 IPlugin，按 shunnet/Daq 约定）。</summary>
public interface IMq : IAsyncDisposable
{
    string Id { get; }
    string Name { get; }
    ValueTask OnStartAsync(IPluginContext ctx, CancellationToken ct);
    ValueTask OnStopAsync(CancellationToken ct);
    ValueTask PublishAsync(string topic, byte[] payload, CancellationToken ct);
}

/// <summary>将宿主 ILogger 桥接到 Contracts.IPluginLogger（不依赖宿主日志实现细节）。</summary>
public sealed class PluginLoggerBridge : IPluginLogger
{
    private readonly ILogger _logger;
    public PluginLoggerBridge(ILogger logger) => _logger = logger;
    public void Log(string message) => _logger.LogInformation("{Msg}", message);
    public void Warn(string message) => _logger.LogWarning("{Msg}", message);
    public void Error(string message, Exception? ex = null) => _logger.LogError(ex, "{Msg}", message);
}

/// <summary>宿主侧 IPluginContext 实现：通过 Contracts 暴露宿主服务给插件。</summary>
public sealed class PluginContext : IPluginContext
{
    public IServiceProvider Services { get; }
    public IPluginLogger Logger { get; }
    public string PluginDirectory { get; }
    public CancellationToken AppStopping { get; }

    public PluginContext(IServiceProvider services, ILogger hostLogger, string directory, CancellationToken stopping)
    {
        Services = services;
        Logger = new PluginLoggerBridge(hostLogger);
        PluginDirectory = directory;
        AppStopping = stopping;
    }
}

/// <summary>§5 插件加载器后台服务：ZIP → 解压 → ALC 流式加载 → FileSystemWatcher 热重载。</summary>
public sealed class PluginLoaderHost : BackgroundService
{
    private readonly ILogger<PluginLoaderHost> _log;
    private readonly IDeviceCatalog _catalog;
    private readonly IServiceProvider _services;
    private readonly IHostApplicationLifetime _life;
    private readonly ConcurrentDictionary<string, PluginLease> _leases = new();
    private readonly ConcurrentDictionary<string, AssemblyLoadContext> _alcs = new();
    private readonly BusProducer _bus;
    private readonly MVT.JoinableTaskFactory _jtf;

    public PluginLoaderHost(
        ILogger<PluginLoaderHost> log,
        IDeviceCatalog catalog,
        IServiceProvider services,
        IHostApplicationLifetime life,
        BusProducer bus,
        MVT.JoinableTaskContext jtc)
    {
        _log = log; _catalog = catalog; _services = services;
        _life = life; _bus = bus; _jtf = jtc.Factory;
    }

    protected override async Task ExecuteAsync(CancellationToken stopping)
    {
        Directory.CreateDirectory(PlcVsaConfig.PLUGINS_DIR);
        var watcher = new FileSystemWatcher(PlcVsaConfig.PLUGINS_DIR, "*.zip")
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            EnableRaisingEvents = true,
        };
        watcher.Created += (_, e) => _ = LoadZipSafeAsync(e.FullPath, stopping);

        foreach (var zip in Directory.EnumerateFiles(PlcVsaConfig.PLUGINS_DIR, "*.zip", SearchOption.AllDirectories))
            await LoadZipSafeAsync(zip, stopping);

        RegisterBuiltInProtocols();
        _log.LogInformation("PluginLoaderHost 启动完毕，监听目录: {Dir}", Path.GetFullPath(PlcVsaConfig.PLUGINS_DIR));
        await Task.Delay(Timeout.Infinite, stopping).ContinueWith(_ => { }, stopping);
    }

    private void RegisterBuiltInProtocols()
    {
        _catalog.Register(new SiemensS7Protocol(DeviceModel.Siemens_S7_1200));
        _catalog.Register(new SiemensS7Protocol(DeviceModel.Siemens_S7_1500));
        _catalog.Register(new SiemensS7Protocol(DeviceModel.Siemens_S7_300));
        _catalog.Register(new ModbusTcpProtocol());
        _catalog.Register(new MelsecMcProtocol());
        _catalog.Register(new OmronFinsProtocol());
        _log.LogInformation("内置协议注册完毕，共 {N} 个", _catalog.All.Count);
    }

    private Task LoadZipSafeAsync(string zip, CancellationToken ct) => _jtf.RunAsync(async () =>
    {
        try
        {
            var id = Path.GetFileNameWithoutExtension(zip);
            var extractDir = Path.Combine(PlcVsaConfig.PLUGINS_DIR, "_extracted", id);
            Directory.CreateDirectory(extractDir);
            ZipFile.ExtractToDirectory(zip, extractDir, overwriteFiles: true);

            var dll = Directory.EnumerateFiles(extractDir, "*.dll", SearchOption.AllDirectories)
                .FirstOrDefault(d => Path.GetFileNameWithoutExtension(d)
                    .Equals(id, StringComparison.OrdinalIgnoreCase));
            if (dll is null) { _log.LogWarning("ZIP {Zip} 中未找到同名 DLL，跳过", zip); return; }
            await LoadPluginAssembly(id, dll, ct);
        }
        catch (Exception ex) { _log.LogError(ex, "加载插件 ZIP 失败: {Zip}", zip); }
    }).Task;

    private async Task LoadPluginAssembly(string id, string dll, CancellationToken ct)
    {
        var alc = new PluginLoadContext(Path.GetDirectoryName(dll)!);
        _alcs[id] = alc;
        var bytes = await File.ReadAllBytesAsync(dll, ct);
        var pdb = Path.ChangeExtension(dll, ".pdb");
        Assembly asm;
        if (pdb is not null && File.Exists(pdb))
            asm = alc.LoadFromStream(new MemoryStream(bytes), new MemoryStream(await File.ReadAllBytesAsync(pdb, ct)));
        else
            asm = alc.LoadFromStream(new MemoryStream(bytes));

        var types = asm.GetTypes().ToList();
        var pluginType = types.FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract);
        var daqType    = types.FirstOrDefault(t => typeof(IDaq).IsAssignableFrom(t)    && !t.IsAbstract);
        var mqType     = types.FirstOrDefault(t => typeof(IMq).IsAssignableFrom(t)     && !t.IsAbstract);
        var protoTypes = types.Where(t => typeof(IDeviceProtocol).IsAssignableFrom(t)  && !t.IsAbstract).ToList();

        foreach (var pt in protoTypes)
        {
            if (Activator.CreateInstance(pt) is IDeviceProtocol proto)
            {
                _catalog.Register(proto);
                _bus.PublishPluginEvent(proto.ProtocolId, "registered");
            }
        }

        var logger = _services.GetRequiredService<ILoggerFactory>().CreateLogger("Plugin:" + id);
        var ctx = new PluginContext(_services, logger, Path.GetDirectoryName(dll)!, _life.ApplicationStopping);

        IPlugin? plugin = null; IDaq? daq = null; IMq? mq = null;
        if (pluginType is not null)
        {
            plugin = (IPlugin)Activator.CreateInstance(pluginType)!;
            await plugin.StartAsync(ctx, ct);
        }
        if (daqType is not null)
        {
            daq = (IDaq)Activator.CreateInstance(daqType)!;
            await daq.OnStartAsync(ctx, ct);
        }
        if (mqType is not null)
        {
            mq = (IMq)Activator.CreateInstance(mqType)!;
            await mq.OnStartAsync(ctx, ct);
        }

        _leases[id] = new PluginLease(plugin, daq, mq, alc);
        _bus.PublishPluginEvent(id, "loaded");
        _log.LogInformation("插件 {Id} 加载完成 (Plugin={P}, DAQ={D}, MQ={M}, Protocols={N})",
            id, plugin is not null, daq is not null, mq is not null, protoTypes.Count);
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        foreach (var (id, lease) in _leases)
        {
            try { await lease.DisposeAsync(); }
            catch (Exception ex) { _log.LogError(ex, "卸载插件 {Id} 异常", id); }
            if (_alcs.TryRemove(id, out var alc)) alc.Unload();
            _bus.PublishPluginEvent(id, "unloaded");
        }
        _leases.Clear();
        await base.StopAsync(ct);
    }
}

public sealed class PluginLease : IAsyncDisposable
{
    private IPlugin? _plugin; private IDaq? _daq; private IMq? _mq; private AssemblyLoadContext _alc;
    public PluginLease(IPlugin? p, IDaq? d, IMq? m, AssemblyLoadContext alc)
    { _plugin = p; _daq = d; _mq = m; _alc = alc; }

    public async ValueTask DisposeAsync()
    {
        if (_plugin is not null) { try { await _plugin.StopAsync(CancellationToken.None); } catch { } _plugin = null; }
        if (_daq is not null) { await _daq.DisposeAsync(); _daq = null; }
        if (_mq  is not null) { await _mq.DisposeAsync();  _mq  = null; }
        try { _alc.Unload(); } catch { /* 忽略 ALC 重复卸载 */ }
    }
}

/// <summary>§5 原生 ALC + AssemblyDependencyResolver（兼容 McMaster 包）。</summary>
public sealed class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;
    public PluginLoadContext(string pluginPath, bool isCollectible = true) : base(isCollectible)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var shared = Assemblies.FirstOrDefault(a => a.GetName().Name == assemblyName.Name);
        if (shared != null) return shared;
        var path = _resolver.ResolveAssemblyToPath(assemblyName);
        return path != null ? LoadFromAssemblyPath(path) : null;
    }
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var p = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return p != null ? LoadUnmanagedDllFromPath(p) : IntPtr.Zero;
    }
}

/// <summary>§9 AsyncLazy 封装：重型资源按 JTF 异步延迟创建。</summary>
public sealed class HeavyResource<T> where T : class
{
    private readonly MVT.AsyncLazy<T> _lazy;
    public HeavyResource(Func<Task<T>> factory, MVT.JoinableTaskFactory jtf)
    {
        _lazy = new MVT.AsyncLazy<T>(factory, jtf);
    }
    public Task<T> GetValueAsync(CancellationToken ct = default) => _lazy.GetValueAsync(ct);
}
