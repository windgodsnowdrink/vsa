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

// 1. Keycloak适配器(高性能实现)
[SkipLocalsInit]
/// <summary>
/// Keycloak服务实现（生产级优化）
/// </summary>
/// <summary>
/// Keycloak服务实现（生产级优化）
/// 集成：零拷贝Span优化、线程本地缓存、断路器、分布式缓存和OpenTelemetry
/// </summary>
public sealed class KeycloakService : IKeycloakService, IDisposable
{
    private readonly KeycloakOptions _options;
    private readonly ILogger<KeycloakService> _logger;
    private readonly Meter _meter = new("Keycloak");
    private readonly KeycloakClient _client;
    private readonly IMemoryCache _cache;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly ObjectPool<Memory<byte>> _spanPool;
    private readonly ThreadLocal<Memory<byte>> _threadLocalSpan;
    private readonly Counter<int> _requestCounter;
    private readonly Histogram<double> _latencyHistogram;
{
    private readonly KeycloakOptions _options;
    private readonly ILogger<KeycloakService> _logger;
    private readonly Meter _meter = new("Keycloak");
    private readonly KeycloakClient _client;
    private readonly IMemoryCache _cache;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    
    public KeycloakService(
        IOptions<KeycloakOptions> options,
        ILogger<KeycloakService> logger,
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ObjectPoolProvider poolProvider)
    {
        _options = options.Value;
        _logger = logger;
        _cache = cache;
        
        // 初始化Span对象池（CPU缓存行对齐）
        _spanPool = poolProvider.Create(new DefaultPooledObjectPolicy<Memory<byte>>()
        {
            public override Memory<byte> Create() => new byte[_options.SpanBufferSize];
        });
        
        // 线程本地Span（零拷贝优化）
        _threadLocalSpan = new ThreadLocal<Memory<byte>>(
            () => _spanPool.Get(),
            trackAllValues: true);
        
        // OpenTelemetry指标
        _requestCounter = _meter.CreateCounter<int>("keycloak.requests", "count");
        _latencyHistogram = _meter.CreateHistogram<double>("keycloak.latency", "ms");
        IOptions<KeycloakOptions> options,
        ILogger<KeycloakService> logger,
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache)
    {
        _options = options.Value;
        _logger = logger;
        _cache = cache;
        
        // 初始化断路器
        _circuitBreaker = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                _options.CircuitBreakerFailuresBeforeBreaking,
                _options.CircuitBreakerDurationOfBreak,
                (ex, breakDelay) => _logger.LogWarning(ex, "Circuit breaker opened for {BreakDelay}ms", breakDelay.TotalMilliseconds),
                () => _logger.LogInformation("Circuit breaker reset"));
                
        var httpClient = httpClientFactory.CreateClient();
        httpClient.Timeout = _options.Timeout;
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            
        _client = new KeycloakClient(
            _options.AuthServerUrl,
            _options.Username,
            _options.Password,
            _options.Realm,
            _options.ClientId,
            _options.ClientSecret,
            httpClient);
{
    private readonly Channel<AuthRequest> _requestChannel;
    private readonly ObjectPool<KeycloakContext> _contextPool;
    private readonly TailLatencyOptimizer _latencyOptimizer;
    private readonly KeycloakClient _keycloakClient;
    private readonly IDistributedCache _cache;

    public KeycloakAdapter(
        KeycloakClient keycloakClient,
        IDistributedCache cache)
    {
        _keycloakClient = keycloakClient;
        _cache = cache;
        _latencyOptimizer = new TailLatencyOptimizer();
        
        // Disruptor模式通道配置
        _requestChannel = Channel.CreateBounded<AuthRequest>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 上下文对象池
        _contextPool = new DefaultObjectPool<KeycloakContext>(
            new KeycloakContextPooledPolicy(), 1000);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<AuthResult> ProcessRequestAsync(AuthRequest request)
    {
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
                await context.ProcessAsync(request, _keycloakClient, _cache);
                _latencyOptimizer.RecordLatency();
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }
}

// 2. Keycloak配置
/// <summary>
/// Keycloak配置选项（生产级优化）
/// </summary>
public sealed class KeycloakOptions
{
    public string AuthServerUrl { get; set; } = "http://localhost:8080";
    public string Realm { get; set; } = "master";
    public string ClientId { get; set; } = "admin-cli";
    public string ClientSecret { get; set; } = string.Empty;
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "admin";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(200);
    
    // 生产级优化配置
    public bool EnableCircuitBreaker { get; set; } = true;
    public int CircuitBreakerFailuresBeforeBreaking { get; set; } = 5;
    public TimeSpan CircuitBreakerDurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
    public bool EnableDistributedCaching { get; set; } = true;
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(5);
{
    /// <summary>
/// Keycloak扩展方法（生产级优化）
/// </summary>
/// <summary>
/// Keycloak扩展方法（生产级优化）
/// 集成：Polly重试策略、断路器、健康检查、分布式缓存和OpenTelemetry
/// </summary>
public static class KeycloakExtensions
{
    /// <summary>
    /// 添加Keycloak服务（生产级优化）
    /// </summary>
    public static IServiceCollection AddKeycloak(this IServiceCollection services, Action<KeycloakOptions> configure)
    {
        services.Configure(configure);
        
        // 添加带Polly重试策略和断路器的HttpClient
        services.AddHttpClient<KeycloakClient>()
            .AddPolicyHandler((provider, _) => 
                Policy.Handle<HttpRequestException>()
                    .WaitAndRetryAsync(
                        provider.GetRequiredService<IOptions<KeycloakOptions>>().Value.MaxRetryAttempts,
                        _ => provider.GetRequiredService<IOptions<KeycloakOptions>>().Value.RetryDelay,
                        onRetry: (ex, delay) => provider.GetRequiredService<ILogger<KeycloakClient>>()
                            .LogWarning(ex, "Retrying Keycloak request after {Delay}ms", delay.TotalMilliseconds)))
            .AddPolicyHandler((provider, _) =>
                provider.GetRequiredService<IOptions<KeycloakOptions>>().Value.EnableCircuitBreaker
                    ? Policy.Handle<HttpRequestException>()
                        .CircuitBreakerAsync(
                            provider.GetRequiredService<IOptions<KeycloakOptions>>().Value.CircuitBreakerFailuresBeforeBreaking,
                            provider.GetRequiredService<IOptions<KeycloakOptions>>().Value.CircuitBreakerDurationOfBreak,
                            onBreak: (ex, breakDelay) => provider.GetRequiredService<ILogger<KeycloakClient>>()
                                .LogWarning(ex, "Circuit breaker opened for {BreakDelay}ms", breakDelay.TotalMilliseconds),
                            onReset: () => provider.GetRequiredService<ILogger<KeycloakClient>>()
                                .LogInformation("Circuit breaker reset"))
                    : Policy.NoOpAsync<HttpResponseMessage>());
{
    /// <summary>
    /// 添加Keycloak服务（生产级优化）
    /// </summary>
    public static IServiceCollection AddKeycloak(this IServiceCollection services, Action<KeycloakOptions> configure)
    {
        services.Configure(configure);
        
        // 添加带Polly重试策略和断路器的HttpClient
        services.AddHttpClient<KeycloakClient>()
            .AddPolicyHandler((provider, _) => 
                Policy.Handle<HttpRequestException>()
                    .WaitAndRetryAsync(
                        provider.GetRequiredService<IOptions<KeycloakOptions>>().Value.MaxRetryAttempts,
                        _ => provider.GetRequiredService<IOptions<KeycloakOptions>>().Value.RetryDelay))
            .AddPolicyHandler((provider, _) =>
                provider.GetRequiredService<IOptions<KeycloakOptions>>().Value.EnableCircuitBreaker
                    ? Policy.Handle<HttpRequestException>()
                        .CircuitBreakerAsync(
                            provider.GetRequiredService<IOptions<KeycloakOptions>>().Value.CircuitBreakerFailuresBeforeBreaking,
                            provider.GetRequiredService<IOptions<KeycloakOptions>>().Value.CircuitBreakerDurationOfBreak)
                    : Policy.NoOpAsync<HttpResponseMessage>());
        
        services.AddSingleton<IKeycloakService, KeycloakService>();
        services.AddHealthChecks().AddCheck<KeycloakHealthCheck>("keycloak");
        
        // 添加分布式缓存（如果启用）
        var options = services.BuildServiceProvider().GetRequiredService<IOptions<KeycloakOptions>>().Value;
        if (options.EnableDistributedCaching)
        {
            services.AddDistributedMemoryCache();
        }
        
        // 添加OpenTelemetry指标
        services.AddOpenTelemetry()
            .WithMetrics(builder => builder
                .AddMeter("Keycloak")
                .AddPrometheusExporter());
                
        return services;
    {
        services.AddSingleton<KeycloakClient>(_ => new KeycloakClient(
            config["Keycloak:ServerUrl"],
            config["Keycloak:Realm"],
            config["Keycloak:ClientId"],
            config["Keycloak:ClientSecret"]));

        services.AddSingleton<KeycloakAdapter>();
        services.AddHostedService<KeycloakAdapter>();

        return services;
    }
}

// 3. 主程序配置
var builder = WebApplication.CreateBuilder(args);

// 配置Redis缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Keycloak:";
});

// 添加Keycloak服务
builder.Services.AddKeycloakServices(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Keycloak SSO Service Ready");
app.Run();

// 辅助记录类型
public record AuthRequest(string ClientId, string Scope);
public record AuthResult(string? Token = null, AuthStatus Status = AuthStatus.Pending);
public enum AuthStatus { Pending, Processing, Completed, Failed }