#:sdk Microsoft.NET.Sdk.Web
#:package Yarp.ReverseProxy@2.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.Caching.Distributed;
using Yarp.ReverseProxy.Configuration;

[SkipLocalsInit]
public sealed class BffGatewayService : IAsyncDisposable
{
    private readonly Channel<ProxyRequest> _requestChannel;
    private readonly ObjectPool<IProxyConfigProvider> _configPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly CancellationTokenSource _cts;
    private readonly IDistributedCache _cache;

    public BffGatewayService(
        IProxyConfigProvider configProvider, 
        IDistributedCache cache)
    {
        _latencyOptimizer = new TailLatencyOptimizer();
        _cts = new CancellationTokenSource();
        _cache = cache;
        
        _requestChannel = Channel.CreateBounded<ProxyRequest>(
            new BoundedChannelOptions(10_000)
            {
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.DropOldest
            });
        
        _configPool = new DefaultObjectPool<IProxyConfigProvider>(
            new ProxyConfigPooledPolicy(configProvider), 
            Environment.ProcessorCount * 2);
        
        _ = Task.Run(ProcessRequestsAsync);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<ProxyResponse> ProcessRequestAsync(ProxyRequest request)
    {
        await _requestChannel.Writer.WriteAsync(request, _cts.Token);
        return await request.Completion.Task;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessRequestsAsync()
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(_cts.Token))
        {
            using var latencyToken = _latencyOptimizer.BeginOperation();
            var configProvider = _configPool.Get();
            try
            {
                var result = await ProcessProxy(configProvider, request);
                request.Completion.SetResult(result);
            }
            finally
            {
                _configPool.Return(configProvider);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task<ProxyResponse> ProcessProxy(
        IProxyConfigProvider configProvider, 
        ProxyRequest request)
    {
        // 使用Span<T>实现零拷贝处理
        using var buffer = MemoryPool<byte>.Shared.Rent(4096);
        var memory = buffer.Memory;
        
        // 实现基于TokenRing的缓冲区管理
        var response = new ProxyResponse();
        // ... 实际代理逻辑实现
        
        return response;
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _requestChannel.Writer.Complete();
        await _requestChannel.Reader.Completion;
    }
}

[SkipLocalsInit]
internal sealed class ProxyConfigPooledPolicy : PooledObjectPolicy<IProxyConfigProvider>
{
    private readonly IProxyConfigProvider _provider;

    public ProxyConfigPooledPolicy(IProxyConfigProvider provider) => _provider = provider;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override IProxyConfigProvider Create() => _provider;

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public override bool Return(IProxyConfigProvider obj) => true;
}

var builder = WebApplication.CreateBuilder(args);

// YARP配置
builder.Services.AddReverseProxy()
    .LoadFromMemory(
        new[]
        {
            new RouteConfig
            {
                RouteId = "api",
                ClusterId = "cluster1",
                Match = new RouteMatch { Path = "/api/{**catch-all}" }
            }
        },
        new[]
        {
            new ClusterConfig
            {
                ClusterId = "cluster1",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "destination1", new DestinationConfig { Address = "http://localhost:5001" } }
                }
            }
        });

// Redis缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost";
    options.InstanceName = "BffGateway_";
});

builder.Services.AddSingleton<BffGatewayService>();

var app = builder.Build();

// 中间件管道
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy(proxyPipeline =>
    {
        proxyPipeline.Use(async (context, next) =>
        {
            // 前置处理
            await next();
            // 后置处理
        });
    });
});

app.MapGet("/", () => "BFF Gateway Service");
app.Run();