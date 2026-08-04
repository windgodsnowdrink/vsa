#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package DotNetCorePlugins@2.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Runtime.Loader;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using DotNetCorePlugins;


/* bash
# 加载插件
POST /plugin/load
Body: "todo-plugin"

# 执行插件
POST /plugin/execute
Body: {
    "ModuleName": "todo",
    "Payload": {"Operation": "create", "Item": {...}}
}
*/
// 1. 插件接口定义
public interface IHotPlugModule : IDisposable
{
    string ModuleName { get; }
    Task InitializeAsync(IServiceProvider services);
    Task ExecuteAsync(object payload);
}

// 2. 插件加载器(Disruptor模式)
[SkipLocalsInit]
public sealed class PluginLoader : BackgroundService
{
    private readonly Channel<PluginMessage> _messageChannel;
    private readonly ObjectPool<PluginContext> _contextPool;
    private readonly IServiceProvider _services;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly Dictionary<string, IHotPlugModule> _modules = new();
    private readonly PluginLoaderOptions _options;

    public PluginLoader(IServiceProvider services, PluginLoaderOptions options)
    {
        _services = services;
        _options = options;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        _messageChannel = Channel.CreateBounded<PluginMessage>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        _contextPool = new DefaultObjectPool<PluginContext>(
            new PluginContextPooledPolicy(), 1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task SendMessageAsync(PluginMessage message)
    {
        await _messageChannel.Writer.WriteAsync(message);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _messageChannel.Reader.ReadAllAsync(stoppingToken))
        {
            var context = _contextPool.Get();
            try
            {
                _latencyOptimizer.Optimize(() => 
                {
                    context.Process(message, _modules);
                });
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }

    public async Task LoadPluginAsync(string pluginPath)
    {
        var loader = PluginLoader.CreateFromAssemblyFile(
            pluginPath,
            config => config.PreferSharedTypes = true);
        
        var moduleType = loader.LoadDefaultAssembly()
            .GetTypes()
            .FirstOrDefault(t => typeof(IHotPlugModule).IsAssignableFrom(t));
        
        if (moduleType != null)
        {
            var module = (IHotPlugModule)ActivatorUtilities
                .CreateInstance(_services, moduleType);
            
            await module.InitializeAsync(_services);
            _modules.Add(module.ModuleName, module);
        }
    }

    // 卸载时调用模块的Dispose方法
    public void UnloadPlugin(string moduleName)
    {
        if (_modules.TryGetValue(moduleName, out var module))
        {
            module.Dispose();
            _modules.Remove(moduleName);
        }
    }
}

// 3. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 配置插件选项
builder.Services.Configure<PluginLoaderOptions>(opt => 
{
    opt.PluginDirectory = "plugins";
});

// 注册插件加载器
builder.Services.AddSingleton<PluginLoader>();
builder.Services.AddHostedService<PluginLoader>();

var app = builder.Build();

// 插件管理端点
app.MapPost("/plugin/load", async (string pluginName, PluginLoader loader) =>
{
    var pluginPath = Path.Combine("plugins", $"{pluginName}.dll");
    await loader.LoadPluginAsync(pluginPath);
    return Results.Ok();
});

app.MapPost("/plugin/execute", async (PluginMessage msg, PluginLoader loader) =>
{
    await loader.SendMessageAsync(msg);
    return Results.Ok();
});

app.Run();

// 4. 辅助类
public record PluginMessage(string ModuleName, object Payload);
public record PluginLoaderOptions
{
    public string PluginDirectory { get; set; }
}

public class PluginContext
{
    public void Process(PluginMessage message, Dictionary<string, IHotPlugModule> modules)
    {
        if (modules.TryGetValue(message.ModuleName, out var module))
        {
            module.ExecuteAsync(message.Payload).Wait();
        }
    }
}

public class PluginContextPooledPolicy : IPooledObjectPolicy<PluginContext>
{
    public PluginContext Create() => new PluginContext();
    public bool Return(PluginContext obj) => true;
}