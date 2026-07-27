#:sdk Microsoft.NET.Sdk.Web
#:package Rougamo@2.3.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package System.Threading.Channels@8.0.0
#:package MemoryPack@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Runtime.CompilerServices;
using Rougamo;
using System.Threading.Channels;
using System.Buffers;
using MemoryPack;
using Microsoft.Extensions.ObjectPool;
using System.Runtime.Loader;
using System.Diagnostics;

// 1. 增强插件元数据模型(支持卸载状态)
[MemoryPackable]
[StructLayout(LayoutKind.Sequential, Pack = 64)]
public partial record PluginMetadata(
    string PluginId,
    string Version,
    DateTimeOffset LoadTime,
    ReadOnlyMemory<byte> Manifest,
    bool IsUnloaded = false);

// 2. 插件服务域上下文(隔离加载)
[SkipLocalsInit]
public sealed class PluginAssemblyLoadContext : AssemblyLoadContext, IDisposable
{
    private readonly string _pluginId;
    private readonly WeakReference<PluginManagerService> _managerRef;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public PluginAssemblyLoadContext(string pluginId, PluginManagerService manager) 
        : base(pluginId, isCollectible: true)
    {
        _pluginId = pluginId;
        _managerRef = new WeakReference<PluginManagerService>(manager);
        _latencyOptimizer = new TailLatencyOptimizer();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // 1. 自定义加载逻辑实现
        var pluginPath = Path.Combine(AppContext.BaseDirectory, "plugins", "cale", "Cale.dll");
        if (File.Exists(pluginPath))
        {
            using var file = File.OpenRead(pluginPath);
            var pdbPath = Path.ChangeExtension(pluginPath, ".pdb");
            if (File.Exists(pdbPath))
            {
                using var pdbFile = File.OpenRead(pdbPath);
                return LoadFromStream(file, pdbFile);
            }
            return LoadFromStream(file);
        }
        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task HandleLoadCommand(PluginLoadRequest request, PluginContext ctx)
    {
        // 2. 实际加载逻辑实现
        var pluginDir = Path.Combine(AppContext.BaseDirectory, "plugins", request.PluginId.ToLower());
        var loadContext = new PluginAssemblyLoadContext(request.PluginId, this);
        
        // 加载主程序集
        var mainAssembly = loadContext.LoadFromAssemblyPath(
            Path.Combine(pluginDir, $"{request.PluginId}.dll"));
        
        // 初始化插件
        var pluginType = mainAssembly.GetTypes()
            .FirstOrDefault(t => typeof(IPluginUnloadable).IsAssignableFrom(t));
        if (pluginType != null)
        {
            var plugin = (IPluginUnloadable)ActivatorUtilities
                .CreateInstance(_services, pluginType);
            
            _loadedPlugins[request.PluginId] = (loadContext, new WeakReference(plugin));
            ctx.PluginId = request.PluginId;
            ctx.Version = request.Version;
            OnPluginLoaded(ctx);
        }
    }

    [PluginInterceptor]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void OnPluginLoaded(PluginContext context)
    {
        // 3. 插件加载后的拦截处理
        var metadata = new PluginMetadata(
            context.PluginId,
            context.Version,
            DateTimeOffset.UtcNow,
            ReadOnlyMemory<byte>.Empty);
        
        // 记录加载日志
        _ = PluginLogChannel.Instance.Writer.TryWriteAsync(
            MemoryPackSerializer.Serialize(metadata));
    }

    [PluginInterceptor]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void OnPluginUnloaded(PluginContext context)
    {
        // 4. 插件卸载后的拦截处理
        var metadata = new PluginMetadata(
            context.PluginId,
            context.Version,
            DateTimeOffset.UtcNow,
            ReadOnlyMemory<byte>.Empty,
            IsUnloaded: true);
        
        // 记录卸载日志
        _ = PluginLogChannel.Instance.Writer.TryWriteAsync(
            MemoryPackSerializer.Serialize(metadata));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal void OnPluginUnloaded(string pluginId)
    {
        // 5. GC完成后的回调实现
        try
        {
            var logPath = Path.Combine(
                AppContext.BaseDirectory, 
                "logs", 
                $"plugin_{pluginId}_unloaded.log");
            
            File.AppendAllText(logPath, 
                $"[{DateTime.UtcNow:O}] Plugin {pluginId} GC completed\n");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"GC callback error: {ex.Message}");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public new void Unload()
    {
        // 触发GC收集前清理资源
        Dispose();
        base.Unload();
        
        // 通知插件管理器
        if (_managerRef.TryGetTarget(out var manager))
        {
            manager.OnPluginUnloaded(_pluginId);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        // 释放非托管资源
    }
}

// 3. 增强型插件拦截器(支持卸载检测)
[AttributeUsage(AttributeTargets.Method)]
public class PluginInterceptorAttribute : MoAttribute
{
    private static readonly ObjectPool<PluginContext> _contextPool = 
        new DefaultObjectPool<PluginContext>(new PluginContextPooledPolicy());
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override void OnEntry(MethodContext context)
    {
        var ctx = _contextPool.Get();
        try
        {
            if (context.Target is IPluginUnloadable { IsUnloaded: true })
            {
                throw new PluginUnloadedException(ctx.PluginId);
            }
            
            ctx.MethodName = context.Method.Name;
            ctx.PluginId = context.Target.GetType().Assembly.GetName().Name;
            
            LogToChannel(MemoryPackSerializer.Serialize(ctx));
        }
        finally
        {
            _contextPool.Return(ctx);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static unsafe void LogToChannel(ReadOnlyMemory<byte> message)
    {
        fixed (byte* ptr = message.Span)
        {
            _ = PluginLogChannel.Instance.Writer.TryWrite(new ReadOnlySpan<byte>(ptr, message.Length));
        }
    }
}

// 4. 插件管理器服务(集成加载/卸载)
[SkipLocalsInit]
public sealed class PluginManagerService : BackgroundService
{
    private readonly Channel<PluginCommand> _commandChannel;
    private readonly ConcurrentDictionary<string, (PluginAssemblyLoadContext, WeakReference)> _loadedPlugins;
    private readonly ObjectPool<PluginContext> _contextPool;
    private readonly IServiceProvider _services;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public PluginManagerService(IServiceProvider services)
    {
        _services = services;
        _latencyOptimizer = new TailLatencyOptimizer();
        _loadedPlugins = new ConcurrentDictionary<string, (PluginAssemblyLoadContext, WeakReference)>();
        
        _commandChannel = Channel.CreateBounded<PluginCommand>(new BoundedChannelOptions(1000)
        {
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });
        
        _contextPool = new DefaultObjectPool<PluginContext>(
            new PluginContextPooledPolicy(), 
            Environment.ProcessorCount * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task LoadPluginAsync(PluginLoadRequest request)
    {
        await _commandChannel.Writer.WriteAsync(new LoadPluginCommand(request));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task UnloadPluginAsync(string pluginId)
    {
        await _commandChannel.Writer.WriteAsync(new UnloadPluginCommand(pluginId));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var command in _commandChannel.Reader.ReadAllAsync(stoppingToken))
        {
            var ctx = _contextPool.Get();
            try
            {
                switch (command)
                {
                    case LoadPluginCommand load:
                        await HandleLoadCommand(load.Request, ctx);
                        break;
                    case UnloadPluginCommand unload:
                        await HandleUnloadCommand(unload.PluginId, ctx);
                        break;
                }
            }
            finally
            {
                _contextPool.Return(ctx);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task HandleUnloadCommand(string pluginId, PluginContext ctx)
    {
        if (_loadedPlugins.TryGetValue(pluginId, out var plugin))
        {
            plugin.Item1.Unload();
            _loadedPlugins.TryRemove(pluginId, out _);
            ctx.PluginId = pluginId;
            OnPluginUnloaded(ctx);
        }
    }

    [PluginInterceptor]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void OnPluginLoaded(PluginContext context)
    {
        // 插件加载后的拦截处理
    }

    [PluginInterceptor]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void OnPluginUnloaded(PluginContext context)
    {
        // 插件卸载后的拦截处理
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    internal void OnPluginUnloaded(string pluginId)
    {
        // GC完成后的回调
    }
}

// 5. 启动配置(集成所有组件)
var builder = WebApplication.CreateBuilder(args);

// 注册插件系统
builder.Services.AddSingleton(PluginLogChannel.Instance);
builder.Services.AddHostedService<PluginManagerService>();
// 在启动配置中注册自定义加载策略
builder.Services.AddPluginLoadStrategy(name => 
    // 自定义加载逻辑，可以从不同位置加载DLL
    Assembly.LoadFrom($"plugins/{name.Name}.dll") 
);

// 配置Rougamo
builder.Services.AddRougamo(opt =>
{
    opt.AddGlobalMo(new PluginInterceptorAttribute());
});

var app = builder.Build();

// 插件管理API
app.MapPost("/plugins/load", async (PluginLoadRequest request, PluginManagerService manager) =>
{
    await manager.LoadPluginAsync(request);
    return Results.Ok();
});

app.MapPost("/plugins/unload/{pluginId}", async (string pluginId, PluginManagerService manager) =>
{
    await manager.UnloadPluginAsync(pluginId);
    return Results.Ok();
});

app.MapGet("/", () => "Enhanced Rougamo Plugin System");
app.Run();

// 上下文对象池策略
[SkipLocalsInit]
internal sealed class PluginContextPooledPolicy : PooledObjectPolicy<PluginContext>
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override PluginContext Create() => new();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override bool Return(PluginContext obj)
    {
        obj.Reset();
        return true;
    }
}

// 插件上下文模型(内存优化)
[StructLayout(LayoutKind.Sequential, Pack = 64)]
public sealed class PluginContext
{
    public string PluginId { get; set; }
    public string Version { get; set; }
    public string MethodName { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Reset()
    {
        PluginId = default;
        Version = default;
        MethodName = default;
    }
}

// 6. 新增命令模型
[MemoryPackable]
public abstract partial record PluginCommand;

[MemoryPackable]
public partial record LoadPluginCommand(PluginLoadRequest Request) : PluginCommand;

[MemoryPackable]
public partial record UnloadPluginCommand(string PluginId) : PluginCommand;

// 7. 插件卸载异常
public class PluginUnloadedException : Exception
{
    public string PluginId { get; }
    
    public PluginUnloadedException(string pluginId) 
        : base($"Plugin {pluginId} has been unloaded")
    {
        PluginId = pluginId;
    }
}

// 8. 可卸载插件接口
public interface IPluginUnloadable
{
    bool IsUnloaded { get; set; }
}

// 新增Cale插件实现示例
public class CalePlugin : IPluginUnloadable
{
    public bool IsUnloaded { get; set; }

    [PluginInterceptor]
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public int Add(int a, int b)
    {
        if (IsUnloaded)
            throw new PluginUnloadedException("Cale");
            
        return a + b;
    }
}

[MethodImpl(MethodImplOptions.AggressiveOptimization)]
protected override Assembly? Load(AssemblyName assemblyName)
{
    // 修改为委托方式的加载逻辑
    return LoadAssemblyWithStrategy(assemblyName, (name) => 
    {
        var pluginPath = Path.Combine(AppContext.BaseDirectory, "plugins", name.Name.ToLower(), $"{name.Name}.dll");
        if (File.Exists(pluginPath))
        {
            using var file = File.OpenRead(pluginPath);
            var pdbPath = Path.ChangeExtension(pluginPath, ".pdb");
            return File.Exists(pdbPath) 
                ? LoadFromStream(file, File.OpenRead(pdbPath)) 
                : LoadFromStream(file);
        }
        return null;
    });
}

[MethodImpl(MethodImplOptions.AggressiveOptimization)]
private Assembly? LoadAssemblyWithStrategy(AssemblyName assemblyName, Func<AssemblyName, Assembly?> loadStrategy)
{
    try
    {
        // 执行委托加载策略
        var assembly = loadStrategy(assemblyName);
        
        // 缓存已加载程序集
        if (assembly != null)
        {
            _loadedAssemblies.TryAdd(assembly.GetName().Name, assembly);
        }
        return assembly;
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Assembly load failed: {ex.Message}");
        return null;
    }
}

// 新增委托定义
public delegate Assembly? AssemblyLoadStrategy(AssemblyName assemblyName);

// 新增扩展方法
[SkipLocalsInit]
public static class PluginLoadExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static IServiceCollection AddPluginLoadStrategy(
        this IServiceCollection services,
        AssemblyLoadStrategy strategy)
    {
        return services.AddSingleton(strategy);
    }
}