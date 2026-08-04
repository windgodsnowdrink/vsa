#:sdk Microsoft.NET.Sdk.Web
#:package SimpleIdServer.OpenID@4.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using SimpleIdServer.OpenID;
using SimpleIdServer.OpenID.Domains;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;

// 1. SSO核心服务(高性能实现)
[SkipLocalsInit]
public sealed class SsoService : BackgroundService
{
    private readonly Channel<SsoRequest> _requestChannel;
    private readonly ObjectPool<SsoContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly IDistributedCache _cache;

    public SsoService(IDistributedCache cache)
    {
        _cache = cache;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _requestChannel = Channel.CreateBounded<SsoRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池
        _contextPool = new DefaultObjectPool<SsoContext>(
            new SsoContextPooledPolicy(), 1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<SsoResponse> ProcessRequestAsync(SsoRequest request)
    {
        await _requestChannel.Writer.WriteAsync(request);
        return new SsoResponse { Status = SsoStatus.Processing };
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(ct))
        {
            var context = _contextPool.Get();
            try
            {
                await context.ProcessAsync(request, _cache);
                _latencyOptimizer.RecordLatency();
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 2. SimpleIdServer配置
public static class SsoConfig
{
    public static IServiceCollection AddSsoServices(this IServiceCollection services)
    {
        services.AddOpenIddict()
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

                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);
            });

        services.AddSingleton<SsoService>();
        services.AddHostedService<SsoService>();

        return services;
    }
}

// 3. 主程序配置
var builder = WebApplication.CreateBuilder(args);

// 添加Redis缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "SSO:";
});

// 添加SSO服务
builder.Services.AddSsoServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "SSO Service Ready");
app.Run();

// 辅助记录类型
public record SsoRequest(string ClientId, string Scope);
public record SsoResponse(string? Token = null, SsoStatus Status = SsoStatus.Pending);
public enum SsoStatus { Pending, Processing, Completed, Failed }