#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Http.Resilience@8.0.0
#:package Microsoft.Extensions.Diagnostics.HealthChecks@8.0.0
#:package Microsoft.Extensions.ObjectPool@8.0.0
#:package OpenTelemetry.Extensions.Hosting@1.7.0
#:property LangVersion preview
#:property TargetFramework net10.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// 1. 高级弹性策略配置
builder.Services.AddHttpClient("resilient-client")
    .AddStandardResilienceHandler(options =>
    {
        // 基础策略
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromMilliseconds(200);
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.MinimumThroughput = 5;
        options.Timeout.Timeout = TimeSpan.FromSeconds(5);

        // 高级策略
        options.Retry.ShouldHandle = args => args.Outcome switch
        {
            { Exception: HttpRequestException } => PredicateResult.True(),
            { Result: HttpResponseMessage response } when (int)response.StatusCode >= 500 => PredicateResult.True(),
            _ => PredicateResult.False()
        };

        // 自适应超时
        options.Timeout.OnTimeout = (args) =>
        {
            var latency = args.Duration.TotalMilliseconds;
            var newTimeout = latency > 1000 ? TimeSpan.FromSeconds(10) : TimeSpan.FromSeconds(5);
            args.Timeout.Timeout = newTimeout;
            return default;
        };

        // 请求缓冲
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
        options.TotalRequestTimeout.OnTimeout = (args) =>
        {
            args.Context.Features.Set(new RequestBufferingFeature());
            return default;
        };
    })
    .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
        MaxConnectionsPerServer = 100
    });

// 2. 健康检查集成
builder.Services.AddHealthChecks()
    .AddCheck<ResilienceHealthCheck>("resilience-check");

// 3. 指标监控和分布式追踪
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddHttpClientInstrumentation()
        .AddMeter("Microsoft.Extensions.Http.Resilience")
        .AddConsoleExporter())
    .WithTracing(tracing => tracing
        .AddHttpClientInstrumentation()
        .AddSource("Microsoft.Extensions.Http.Resilience")
        .AddConsoleExporter());

// 4. 对象池优化
builder.Services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();

// 5. 自适应策略配置
builder.Services.Configure<HttpResilienceOptions>(builder.Configuration.GetSection("Resilience"));

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapGet("/", async (IHttpClientFactory clientFactory) =>
{
    var client = clientFactory.CreateClient("resilient-client");
    var response = await client.GetAsync("https://example.com/api");
    return response.IsSuccessStatusCode 
        ? Results.Ok(await response.Content.ReadAsStringAsync()) 
        : Results.Problem("Service unavailable");
});

app.Run();

// 自定义健康检查实现
public class ResilienceHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _clientFactory;

    public ResilienceHealthCheck(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _clientFactory.CreateClient("resilient-client");
            var response = await client.GetAsync("https://example.com/health");
            return response.IsSuccessStatusCode 
                ? HealthCheckResult.Healthy() 
                : HealthCheckResult.Unhealthy("Service degraded");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message, ex);
        }
    }
}

// 请求缓冲特性
public class RequestBufferingFeature
{
    public bool IsBuffered { get; set; } = true;
}

// 弹性策略配置
public class HttpResilienceOptions
{
    public int MaxRetryAttempts { get; set; } = 3;
    public double CircuitBreakerFailureRatio { get; set; } = 0.5;
    public TimeSpan SamplingDuration { get; set; } = TimeSpan.FromSeconds(30);
    public TimeSpan TimeoutDuration { get; set; } = TimeSpan.FromSeconds(5);
}