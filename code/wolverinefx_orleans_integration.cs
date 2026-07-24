#:package WolverineFx@1.7.0
#:package Microsoft.Orleans.Core@3.7.2
#:package Microsoft.Orleans.OrleansRuntime@3.7.2
#:package System.Threading.Channels@7.0.0
#:property TargetFramework net11.0
#:property Nullable enable

using Wolverine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Concurrency;
using Orleans.Runtime;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Threading.Tasks;

// 1. 定义Orleans Grain接口
public interface IWolverineGrain : IGrainWithGuidKey
{
    Task<TResponse> Send<TResponse>(object request);
    Task Publish(object message);
}

// 2. 实现Wolverine Grain
[Reentrant]
public class WolverineGrain : Grain, IWolverineGrain
{
    private readonly IMessageBus _bus;
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<object>> _pendingRequests = new();

    public WolverineGrain([PersistentState("wolverineState", "wolverineStore")] IPersistentState<WolverineGrainState> state)
    {
        _bus = this.ServiceProvider.GetRequiredService<IMessageBus>();
    }

    public async Task<TResponse> Send<TResponse>(object request)
    {
        return await _bus.InvokeAsync<TResponse>(request);
    }

    public async Task Publish(object message)
    {
        await _bus.PublishAsync(message);
    }

    // 跨Actor消息路由方法
    public async Task<TResponse> RouteToActor<TResponse>(object request, GrainId targetActorId)
    {
        var targetGrain = GrainFactory.GetGrain<IWolverineGrain>(targetActorId);
        return await targetGrain.Send<TResponse>(request);
    }
}

// 3. 定义Orleans Grain状态
public class WolverineGrainState
{
    public List<object> PendingMessages { get; set; } = new();
}

// 4. 分布式Wolverine处理器
public class OrleansMessageHandler<TMessage>
{
    private readonly IGrainFactory _grainFactory;

    public OrleansMessageHandler(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public async Task Handle(TMessage message)
    {
        var wolverineGrains = await GetRelevantWolverineGrains();
        var tasks = wolverineGrains.Select(g => g.Publish(message));
        await Task.WhenAll(tasks);
    }

    private async Task<IEnumerable<IWolverineGrain>> GetRelevantWolverineGrains()
    {
        return Enumerable.Empty<IWolverineGrain>();
    }
}

// 分布式追踪增强
[MemoryPackable]
public partial record CrossGrainMessage(string TraceId, string SpanId, string TargetGrainId, object Payload);

// 5. DI扩展
public static class WolverineOrleansIntegrationExtensions
{
    public static IServiceCollection AddWolverineOrleansIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        // 注册Orleans特定的处理器
        services.AddTransient(typeof(OrleansMessageHandler<>));
        
        // 添加高性能通道用于跨Grain通信
        services.AddSingleton<Channel<object>>(sp => 
            Channel.CreateUnbounded<object>(new UnboundedChannelOptions()
            {
                SingleReader = true,
                SingleWriter = false
            }));
            
        // 配置Orleans序列化优化
        services.Configure<OrleansJsonSerializerOptions>(options =>
        {
            options.JsonSerializerOptions.WriteIndented = false;
            options.JsonSerializerOptions.DefaultBufferSize = 16 * 1024;
        });

        // 添加分布式追踪
        services.AddOpenTelemetry()
            .WithTracing(builder => builder
                .AddSource("Wolverine.Orleans")
                .AddOtlpExporter());

        services.AddSingleton<IIncomingGrainCallFilter, WolverineGrain>();
        services.AddSingleton<HighPerformanceChannel<CrossGrainMessage>>();
        services.AddHostedService<ChannelBackgroundService>();

        return services;
    }

    public static WolverineOptions UseOrleansTransport(this WolverineOptions options, Action<OrleansTransportOptions> configure)
    {
        options.Services.Configure(configure);
        
        // 配置Wolverine使用Orleans传输
        options.UseTransport<OrleansTransport>();
        
        // 配置重试策略
        options.Policies.RetryOnException<OrleansException>()
            .MaximumAttempts(3)
            .Wait(100.Milliseconds(), 1.Seconds(), 5.Seconds());
            
        // 配置死信队列
        options.Policies.OnException<OrleansException>()
            .MoveToErrorQueue();
            
        return options;
    }
}

// Orleans传输实现
public class OrleansTransport : ITransport
{
    private readonly IGrainFactory _grainFactory;
    private readonly OrleansTransportOptions _options;
    
    public OrleansTransport(IGrainFactory grainFactory, IOptions<OrleansTransportOptions> options)
    {
        _grainFactory = grainFactory;
        _options = options.Value;
    }
    
    public async ValueTask InitializeAsync()
    {
        // 初始化连接
    }
    
    public async ValueTask SendAsync(Envelope envelope)
    {
        var targetGrain = _grainFactory.GetGrain<IWolverineGrain>(Guid.Parse(_options.TargetGrainId));
        await targetGrain.Send<object>(envelope.Message);
    }
}

// Program.cs 示例配置
/*
var builder = WebApplication.CreateBuilder(args);

// 配置Orleans
builder.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage("wolverineStore");
});

// 添加Wolverine和Orleans集成
builder.Services.AddWolverine(opts =>
{
    opts.UseOrleansTransport(o =>
    {
        o.TargetGrainId = "default";
    });
});

var app = builder.Build();

app.MapGet("/send-to-actor", async (IMessageBus bus, GrainId actorId, string message) =>
{
    var request = new SampleRequest(message);
    var wolverineGrain = app.Services.GetRequiredService<IGrainFactory>().GetGrain<IWolverineGrain>(actorId);
    var response = await wolverineGrain.Send<SampleResponse>(request);
    return Results.Ok(response);
});

app.Run();
*/

// 客户端示例
/*
// 获取Wolverine Grain
var wolverineGrain = grainFactory.GetGrain<IWolverineGrain>(Guid.NewGuid());

// 发送请求
var response = await wolverineGrain.Send<SampleResponse>(new SampleRequest("Hello from client"));

// 路由到特定Actor
var targetActorId = GrainId.Create("SomeActorType", "actor1");
var routedResponse = await wolverineGrain.RouteToActor<SampleResponse>(new SampleRequest("Routed message"), targetActorId);
*/