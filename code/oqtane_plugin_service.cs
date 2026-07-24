#:sdk Microsoft.NET.Sdk.Web
#:package Oqtane.Framework@4.0.0
#:package Oqtane.SSR@4.0.0
#:package System.Threading.Channels@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using Oqtane.Models;
using Oqtane.Interfaces;
using Oqtane.SSR;
using Microsoft.Extensions.ObjectPool;

// 1. 插件接口定义
public interface IOqtanePlugin : IDisposable
{
    string ModuleName { get; }
    Task InitializeAsync(IServiceProvider services);
    Task<RenderResult> ExecuteAsync(object payload);
}

// 2. 插件加载器(Disruptor模式)
[SkipLocalsInit]
public sealed class OqtanePluginLoader : BackgroundService
{
    private readonly Channel<PluginMessage> _messageChannel;
    private readonly ObjectPool<PluginContext> _contextPool;
    private readonly IServiceProvider _services;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly Dictionary<string, IOqtanePlugin> _modules = new();
    private readonly IModuleRepository _moduleRepository;

    public OqtanePluginLoader(IServiceProvider services, IModuleRepository moduleRepository)
    {
        _services = services;
        _moduleRepository = moduleRepository;
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

    public async Task LoadPluginAsync(Module module)
    {
        var moduleType = Type.GetType(module.ModuleTypeName);
        if (moduleType != null && typeof(IOqtanePlugin).IsAssignableFrom(moduleType))
        {
            var plugin = (IOqtanePlugin)ActivatorUtilities
                .CreateInstance(_services, moduleType);
            
            await plugin.InitializeAsync(_services);
            _modules.Add(plugin.ModuleName, plugin);
            _moduleRepository.AddModule(module);
        }
    }
}

// 3. 主程序集成
var builder = WebApplication.CreateBuilder(args);

// 配置Oqtane
builder.Services.AddOqtane();
builder.Services.AddOqtaneSSR();

// 注册插件加载器
builder.Services.AddSingleton<OqtanePluginLoader>();
builder.Services.AddHostedService<OqtanePluginLoader>();

var app = builder.Build();

// 插件管理端点
app.MapPost("/plugin/load", async (Module module, OqtanePluginLoader loader) =>
{
    await loader.LoadPluginAsync(module);
    return Results.Ok();
});

app.MapPost("/plugin/execute", async (PluginMessage msg, OqtanePluginLoader loader) =>
{
    await loader.SendMessageAsync(msg);
    return Results.Ok();
});

app.Run();

// 4. 辅助类
public record PluginMessage(string ModuleName, object Payload);
public class PluginContext
{
    public void Process(PluginMessage message, Dictionary<string, IOqtanePlugin> modules)
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

// 5. 示例插件实现
public class TodoPlugin : IOqtanePlugin
{
    private readonly List<TodoItem> _todos = new();
    
    public string ModuleName => "todo";
    
    public Task InitializeAsync(IServiceProvider services) 
        => Task.CompletedTask;
    
    public Task<RenderResult> ExecuteAsync(object payload)
    {
        if (payload is TodoCommand cmd)
        {
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
        }
        
        return Task.FromResult(new RenderResult
        {
            Content = JsonSerializer.Serialize(_todos),
            StatusCode = 200
        });
    }
    
    public void Dispose() => _todos.Clear();
}

public record TodoCommand(string Operation, TodoItem Item);
public record TodoItem(Guid Id, string Title, bool IsCompleted);