#:sdk Microsoft.NET.Sdk.Web
#:package Yarp.ReverseProxy@2.0.0
#:package Microsoft.Extensions.Http.Discovery@8.0.0
#:package Microsoft.Extensions.ServiceDiscovery@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Net.Http;
using Microsoft.Extensions.ServiceDiscovery;

[SkipLocalsInit]
public sealed class ServiceRegistryMiddleware
{
    private readonly Channel<ServiceEndpoint> _discoveryChannel;
    private readonly ObjectPool<IServiceDiscoveryClient> _discoveryPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly CancellationTokenSource _cts;

    public ServiceRegistryMiddleware(IServiceDiscoveryClient discoveryClient)
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        _cts = new CancellationTokenSource();
        
        _discoveryChannel = Channel.CreateBounded<ServiceEndpoint>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        _discoveryPool = new DefaultObjectPool<IServiceDiscoveryClient>(
            new DiscoveryClientPooledPolicy(discoveryClient), 
            Environment.ProcessorCount * 2);
        
        _ = Task.Run(ProcessDiscoveriesAsync);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task RegisterEndpointAsync(ServiceEndpoint endpoint)
    {
        await _discoveryChannel.Writer.WriteAsync(endpoint, _cts.Token);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessDiscoveriesAsync()
    {
        await foreach (var endpoint in _discoveryChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var latencyToken = _latencyOptimizer.BeginOperation();
            var client = _discoveryPool.Get();
            try
            {
                await client.RegisterServiceAsync(endpoint);
            }
            finally
            {
                _discoveryPool.Return(client);
            }
        }
    }
}

[SkipLocalsInit]
internal sealed class DiscoveryClientPooledPolicy : PooledObjectPolicy<IServiceDiscoveryClient>
{
    private readonly IServiceDiscoveryClient _client;

    public DiscoveryClientPooledPolicy(IServiceDiscoveryClient client) => _client = client;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override IServiceDiscoveryClient Create() => _client;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override bool Return(IServiceDiscoveryClient obj) => true;
}

var builder = WebApplication.CreateBuilder(args);

// 服务发现配置
builder.Services.AddServiceDiscoveryCore();
builder.Services.AddHttpClient("discovery")
    .UseServiceDiscovery();

// YARP动态配置
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((context, handler) => 
    {
        handler.PooledConnectionLifetime = TimeSpan.FromMinutes(5);
        handler.UseCookies = false;
    });

// 服务注册中间件
builder.Services.AddSingleton<ServiceRegistryMiddleware>();

var app = builder.Build();

// 动态端点注册示例
app.MapPost("/register", async (ServiceRegistryMiddleware registry) =>
{
    await registry.RegisterEndpointAsync(new ServiceEndpoint
    {
        Name = "service1",
        Endpoints = new[] { "http://localhost:5001" }
    });
    return Results.Ok();
});

app.MapReverseProxy();
app.Run();