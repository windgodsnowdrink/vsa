#:sdk Microsoft.NET.Sdk.Web
#:package Keycloak.Net@22.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Keycloak.Net;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

// 1. Keycloak服务(高性能实现)
[SkipLocalsInit]
public sealed class KeycloakService : BackgroundService
{
    private readonly Channel<AuthRequest> _requestChannel;
    private readonly ObjectPool<KeycloakContext> _contextPool;
    private readonly KeycloakClient _keycloakClient;
    private readonly IDistributedCache _cache;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly KeycloakOptions _options;

    public KeycloakService(
        KeycloakClient keycloakClient,
        IDistributedCache cache,
        IOptions<KeycloakOptions> options)
    {
        _keycloakClient = keycloakClient;
        _cache = cache;
        _options = options.Value;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _requestChannel = Channel.CreateBounded<AuthRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池(CPU cache-line对齐)
        _contextPool = new DefaultObjectPool<KeycloakContext>(
            new KeycloakContextPooledPolicy(), 
            Environment.ProcessorCount * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<AuthResult> AuthenticateAsync(string username, string password)
    {
        var cacheKey = $"auth:{username}";
        var cached = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<AuthResult>(cached);
        }

        var request = new AuthRequest(username, password);
        await _requestChannel.Writer.WriteAsync(request);
        return new AuthResult { Status = AuthStatus.Processing };
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var request in _requestChannel.Reader.ReadAllAsync(ct))
        {
            var context = _contextPool.Get();
            try
            {
                var result = await context.ProcessAsync(request, _keycloakClient, _options);
                
                // 缓存结果
                await _cache.SetStringAsync(
                    $"auth:{request.Username}", 
                    JsonSerializer.Serialize(result),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = 
                            TimeSpan.Parse(_options.CacheDuration)
                    });
                
                _latencyOptimizer.RecordLatency();
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 配置选项类
public class KeycloakOptions
{
    public string ServerUrl { get; set; }
    public string Realm { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string CacheDuration { get; set; }
}

// 主程序配置
var builder = WebApplication.CreateBuilder(args);

// 配置Keycloak选项
builder.Services.Configure<KeycloakOptions>(builder.Configuration.GetSection("Keycloak"));

// 配置Redis缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

// 注册Keycloak服务
builder.Services.AddSingleton<KeycloakClient>(sp => 
{
    var options = sp.GetRequiredService<IOptions<KeycloakOptions>>().Value;
    return new KeycloakClient(
        options.ServerUrl,
        options.Realm,
        options.ClientId,
        options.ClientSecret);
});

builder.Services.AddSingleton<KeycloakService>();
builder.Services.AddHostedService<KeycloakService>();

var app = builder.Build();

app.MapPost("/authenticate", async (LoginRequest request, KeycloakService service) =>
{
    var result = await service.AuthenticateAsync(request.Username, request.Password);
    return Results.Ok(result);
});

app.Run();

// 辅助记录类型
public record AuthRequest(string Username, string Password);
public record AuthResult(string? Token = null, AuthStatus Status = AuthStatus.Pending);
public record LoginRequest(string Username, string Password);
public enum AuthStatus { Pending, Processing, Completed, Failed }