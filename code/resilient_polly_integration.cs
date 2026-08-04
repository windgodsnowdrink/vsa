// #:sdk Microsoft.NET.Sdk
// #:package Microsoft.Extensions.Http.Polly@8.0.0
// #:package Polly@8.3.0
// #:property LangVersion preview
// #:property TargetFramework net10.0

using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Bulkhead;
using System.Net;

public class ResilientOptions
    {
        public int RetryCount { get; set; } = 3;
        public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
        public int CircuitBreakerThreshold { get; set; } = 5;
        public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
        public int MaxParallelization { get; set; } = 100;
        public int MaxQueuingActions { get; set; } = 50;
        public bool EnableTelemetry { get; set; } = true;
        public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);
        public Action<CircuitState, CircuitState, Exception>? OnCircuitStateChanged { get; set; }
    }

public interface IResilientService
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action);
}

public class ResilientService : IResilientService
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
        private readonly AsyncBulkheadPolicy _bulkheadPolicy;
        private readonly ConcurrentDictionary<string, ResilienceMetrics> _metrics = new();
        private Timer? _healthCheckTimer;

    public ResilientService(ResilientOptions options)
    {
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                options.RetryCount,
                retryAttempt => options.RetryDelay,
                (exception, delay, retryCount, context) => 
                {
                    Console.WriteLine($"[Retry] Attempt {retryCount} failed. Waiting {delay.TotalMilliseconds}ms before next retry. Exception: {exception.Message}");
                    context["LastRetryTime"] = DateTime.UtcNow;
                });

        _circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .CircuitBreakerAsync(
                options.CircuitBreakerThreshold,
                options.CircuitBreakerDuration,
                (exception, duration) => 
                {
                    Console.WriteLine($"[CircuitBreaker] Circuit opened for {duration.TotalSeconds} seconds due to: {exception.Message}");
                    context["CircuitOpenedTime"] = DateTime.UtcNow;
                },
                () => 
                {
                    Console.WriteLine("[CircuitBreaker] Circuit closed and allowing requests again");
                    context["CircuitClosedTime"] = DateTime.UtcNow;
                });

        _bulkheadPolicy = Policy
            .BulkheadAsync(options.MaxParallelization, options.MaxQueuingActions);
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        var policyWrap = Policy.WrapAsync(_bulkheadPolicy, _circuitBreakerPolicy, _retryPolicy);
        return await policyWrap.ExecuteAsync(action);
    }
}

public static class ResilientExtensions
{
    public static IServiceCollection AddResilientWithPolly(this IServiceCollection services, Action<ResilientOptions> configure)
    {
        services.AddOptions<ResilientOptions>().Configure(configure);
        services.AddSingleton<IResilientService, ResilientService>();
        services.AddHostedService<ResilientBackgroundService>();
        services.AddHealthChecks().AddCheck<ResilientHealthCheck>("resilient");
        return services;
    }
}

public class ResilientBackgroundService : BackgroundService
{
    private readonly IResilientService _resilientService;
    private readonly ResilientOptions _options;

    public ResilientBackgroundService(IResilientService resilientService, IOptions<ResilientOptions> options)
    {
        _resilientService = resilientService;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(_options.HealthCheckInterval, stoppingToken);
            // 执行定期健康检查和指标收集
        }
    }
}

public class ResilientHealthCheck : IHealthCheck
{
    private readonly IResilientService _resilientService;

    public ResilientHealthCheck(IResilientService resilientService)
    {
        _resilientService = resilientService;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        // 基于指标实现健康检查逻辑
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}

public class ResilienceMetrics
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public int RetryCount { get; set; }
    public int CircuitBreakerTripped { get; set; }
    public TimeSpan AverageExecutionTime { get; set; }
}

public class ResilientExample
{
    public static void Demo()
    {
        var services = new ServiceCollection();
        services.AddResilientWithPolly(options => 
        {
            options.RetryCount = 5;
            options.CircuitBreakerThreshold = 3;
            options.MaxParallelization = 200;
        });

        var provider = services.BuildServiceProvider();
        var resilientService = provider.GetRequiredService<IResilientService>();

        var result = resilientService.ExecuteAsync(async () => 
        {
            // 执行HTTP请求或其他可能失败的操作
            return await Task.FromResult("success");
        }).GetAwaiter().GetResult();
    }
}