#:sdk Microsoft.NET.Sdk.Web
#:package NetPro.Plugins@6.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package MemoryPack@2.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using NetPro.Plugins;
using Microsoft.Extensions.ObjectPool;
using MemoryPack;

// 1. 插件接口定义
[MemoryPackable]
public partial interface INetProPlugin : IDisposable
{
    string PluginName { get; }
    Task InitializeAsync(IServiceProvider services);
    Task<PluginResult> ExecuteAsync(ReadOnlyMemory<byte> payload);
}

// 2. 高性能插件引擎(Disruptor模式)
[SkipLocalsInit]
public sealed class PluginEngine : BackgroundService
{
    private readonly Channel<PluginMessage> _messageChannel;
    private readonly ObjectPool<PluginContext> _contextPool;
    private readonly IServiceProvider _services;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly Dictionary<string, INetProPlugin> _plugins = new();
    private readonly PluginOptions _options;

    public PluginEngine(IServiceProvider services, IOptions<PluginOptions> options)
    {
        _services = services;
        _options = options.Value;
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
                    context.Process(message, _plugins);
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
        var loader = new PluginLoader(_options.PluginDirectory);
        var plugin = loader.LoadPlugin<INetProPlugin>(pluginPath);
        
        await plugin.InitializeAsync(_services);
        _plugins.Add(plugin.PluginName, plugin);
    }
}

// 3. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 配置插件选项
builder.Services.Configure<PluginOptions>(opt => 
{
    opt.PluginDirectory = "plugins";
    opt.ShadowCopy = true;
});

// 注册插件引擎
builder.Services.AddSingleton<PluginEngine>();
builder.Services.AddHostedService<PluginEngine>();

var app = builder.Build();

// 插件管理端点
app.MapPost("/plugin/load", async (string pluginName, PluginEngine engine) =>
{
    var pluginPath = Path.Combine("plugins", $"{pluginName}.dll");
    await engine.LoadPluginAsync(pluginPath);
    return Results.Ok();
});

app.MapPost("/plugin/execute", async (PluginMessage msg, PluginEngine engine) =>
{
    await engine.SendMessageAsync(msg);
    return Results.Ok();
});

app.Run();

// 4. 辅助类
[MemoryPackable]
public partial record PluginMessage(string PluginName, ReadOnlyMemory<byte> Payload);

[MemoryPackable]
public partial record PluginResult(int StatusCode, ReadOnlyMemory<byte> Data);

public class PluginContext
{
    public void Process(PluginMessage message, Dictionary<string, INetProPlugin> plugins)
    {
        if (plugins.TryGetValue(message.PluginName, out var plugin))
        {
            plugin.ExecuteAsync(message.Payload).Wait();
        }
    }
}

public class PluginContextPooledPolicy : IPooledObjectPolicy<PluginContext>
{
    public PluginContext Create() => new PluginContext();
    public bool Return(PluginContext obj) => true;
}

// 5. 示例插件实现
[MemoryPackable]
public partial class TodoPlugin : INetProPlugin
{
    private readonly List<TodoItem> _todos = new();
    
    public string PluginName => "todo";
    
    public Task InitializeAsync(IServiceProvider services) 
        => Task.CompletedTask;
    
    public Task<PluginResult> ExecuteAsync(ReadOnlyMemory<byte> payload)
    {
        var cmd = MemoryPackSerializer.Deserialize<TodoCommand>(payload.Span);
        
        switch (cmd.Operation.ToLower())
        {
            case "create":
                _todos.Add(cmd.Item);
                break;
            case "update":
                var existing = _todos.FirstOrDefault(x => x.Id == cmd.Item.Id);
                if (existing != null)
                {
                    _todos.Remove(existing);
                    _todos.Add(cmd.Item);
                }
                break;
            case "delete":
                _todos.RemoveAll(x => x.Id == cmd.Item.Id);
                break;
        }
        
        var result = new PluginResult(200, 
            MemoryPackSerializer.SerializeToMemory(_todos));
        return Task.FromResult(result);
    }
    
    public void Dispose() => _todos.Clear();
}

[MemoryPackable]
public partial record TodoCommand(string Operation, TodoItem Item);

[MemoryPackable]
public partial record TodoItem(Guid Id, string Title, bool IsCompleted);