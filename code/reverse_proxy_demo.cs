#:sdk Microsoft.NET.Sdk.Web
#:package Yarp.ReverseProxy@2.0.0
#:package Microsoft.VisualStudio.Azure.Containers.Tools.Targets@1.19.6
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property UserSecretsId 210f4926-30c7-45ca-a020-391f82b3b3a1
#:property DockerDefaultTargetOS Linux
#:property DockerComposeProjectPath ..\docker-compose.dcproj

using System.Buffers;
using System.Threading.Channels;
using Yarp.ReverseProxy.Configuration;
using System.Threading.Tasks.Dataflow;

// 内存优化相关类
public class MemoryOptimizer
{
    private readonly ObjectPool<Memory<byte>> _memoryPool;
    private readonly ThreadLocal<Span<byte>> _threadLocalSpan;

    public MemoryOptimizer()
    {
        _memoryPool = new DefaultObjectPool<Memory<byte>>(
            new DefaultPooledObjectPolicy<Memory<byte>>(), 1000);
        _threadLocalSpan = new ThreadLocal<Span<byte>>(() => stackalloc byte[1024]);
    }
}

// 代理配置服务
public class ProxyConfigService
{
    private readonly Channel<RouteConfig> _configChannel;
    private readonly MemoryOptimizer _memoryOptimizer;

    public ProxyConfigService(MemoryOptimizer memoryOptimizer)
    {
        _memoryOptimizer = memoryOptimizer;
        _configChannel = Channel.CreateUnbounded<RouteConfig>();
        _ = ProcessConfigUpdatesAsync();
    }

    private async Task ProcessConfigUpdatesAsync()
    {
        await foreach (var config in _configChannel.Reader.ReadAllAsync())
        {
            // 处理配置更新
        }
    }

    public async Task UpdateRouteAsync(RouteConfig config)
    {
        await _configChannel.Writer.WriteAsync(config);
    }
}

var builder = WebApplication.CreateBuilder();

// 添加内存优化服务
builder.Services.AddSingleton<MemoryOptimizer>();
builder.Services.AddSingleton<ProxyConfigService>();

// 配置YARP反向代理
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
                },
                HttpRequest = new ForwarderRequestConfig
                {
                    ActivityTimeout = TimeSpan.FromSeconds(30),
                    Version = new Version(1, 1),
                    VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
                }
            }
        });

var app = builder.Build();

// 使用反向代理中间件
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        // 前置处理
        await next();
        // 后置处理
    });
});

app.Run();