#:sdk Microsoft.NET.Sdk.Web
#:package SimpleIdServer.OpenId@4.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package System.Threading.Channels@8.0.0
#:package MassTransit@8.2.2
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using SimpleIdServer.OpenID;
using MassTransit;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;

// 1. SSO微服务核心(事件溯源+CQRS架构)
[SkipLocalsInit]
public sealed class SsoMicroservice : BackgroundService
{
    private readonly Channel<SsoCommand> _commandChannel;
    private readonly ObjectPool<SsoContext> _contextPool;
    private readonly IBus _bus;
    private readonly IDistributedCache _cache;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public SsoMicroservice(
        IBus bus, 
        IDistributedCache cache)
    {
        _bus = bus;
        _cache = cache;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式命令通道
        _commandChannel = Channel.CreateBounded<SsoCommand>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 对象池配置(CPU cache-line对齐)
        _contextPool = new DefaultObjectPool<SsoContext>(
            new SsoContextPooledPolicy(), 
            Environment.ProcessorCount * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task SendCommandAsync(SsoCommand command)
    {
        await _commandChannel.Writer.WriteAsync(command);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var command in _commandChannel.Reader.ReadAllAsync(ct))
        {
            var context = _contextPool.Get();
            try
            {
                await context.ProcessAsync(command, _cache, _bus);
                _latencyOptimizer.RecordLatency();
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 2. 集成MassTransit事件总线
public static class SsoBusConfig
{
    public static IServiceCollection AddSsoEventBus(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}

// 3. 主程序配置
var builder = WebApplication.CreateBuilder(args);

// 配置Redis缓存(分层内存服务)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "SSO:";
});

// 配置事件总线
builder.Services.AddSsoEventBus();

// 配置SimpleIdServer
builder.Services.AddOpenIddict()
    .AddCore(options => options.UseEntityFrameworkCore())
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token")
            .SetUserinfoEndpointUris("/connect/userinfo");

        options.AllowAuthorizationCodeFlow()
            .AllowPasswordFlow()
            .AllowRefreshTokenFlow();

        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();
    });

// 注册SSO微服务
builder.Services.AddSingleton<SsoMicroservice>();
builder.Services.AddHostedService<SsoMicroservice>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "SSO Microservice Ready");
app.Run();

// 辅助类型定义
public record SsoCommand(string Type, string Payload);
public record SsoEvent(string Type, string Payload);