#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Http.Polly@10.0.0
#:package Polly@7.2.4
#:package Microsoft.Extensions.Options@10.0.0
#:package System.Threading.RateLimiting@8.0.0
#:package Microsoft.Extensions.Http.Resilience@10.0.0
#:package System.Buffers@4.5.1
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property PublishAot=true
#:property TrimMode=partial
#:property EnableCompressionInSingleFile=true
#:property SelfContained=true

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Bulkhead;
using Polly.Timeout;
using System.Threading.RateLimiting;

// 弹性策略选项
public class ResilientOptions
{
    // 重试策略配置
    public int RetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public bool UseExponentialBackoff { get; set; } = true;
    
    // 断路器配置
    public double CircuitBreakerFailureThreshold { get; set; } = 0.5;
    public int CircuitBreakerSamplingDuration { get; set; } = 10;
    public int CircuitBreakerMinimumThroughput { get; set; } = 10;
    public TimeSpan CircuitBreakerDurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
    
    // 限速器配置
    public int RateLimiterPermitLimit { get; set; } = 100;
    public TimeSpan RateLimiterWindow { get; set; } = TimeSpan.FromSeconds(1);
    public int RateLimiterQueueLimit { get; set; } = 50;
    
    // 超时配置
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    
    // 舱壁隔离配置
    public int MaxParallelization { get; set; } = 100;
    public int MaxQueuingActions { get; set; } = 50;
    
    // 健康检查配置
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);
    public bool EnableHealthCheck { get; set; } = true;
    
    // 遥测配置
    public bool EnableTelemetry { get; set; } = true;
}

// 弹性策略接口
public interface IResilientStrategy
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
}

// 弹性服务接口
public interface IResilientService
{
    // 使用重试策略执行操作
    Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 使用断路器模式执行操作
    Task<T> ExecuteWithCircuitBreakerAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 使用限速器执行操作
    Task<T> ExecuteWithRateLimiterAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 使用超时策略执行操作
    Task<T> ExecuteWithTimeoutAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 使用回退策略执行操作
    Task<T> ExecuteWithFallbackAsync<T>(Func<Task<T>> operation, Func<Task<T>> fallback, CancellationToken cancellationToken = default);
    
    // 使用舱壁隔离执行操作
    Task<T> ExecuteWithBulkheadAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 使用组合策略执行操作
    Task<T> ExecuteWithResilienceAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default);
    
    // 获取健康状态
    Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default);
}

// 健康检查结果
public class HealthCheckResult
{
    public bool IsHealthy { get; set; }
    public string Status { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, object> Metrics { get; set; } = new();
}

// 弹性指标
public class ResilienceMetrics
{
    public int TotalOperations { get; set; }
    public int FailedOperations { get; set; }
    public int RetriedOperations { get; set; }
    public int CircuitBreakerOpens { get; set; }
    public int RateLimitedOperations { get; set; }
    public int TimedOutOperations { get; set; }
    public double FailureRate => TotalOperations > 0 ? (double)FailedOperations / TotalOperations : 0;
}

// 基于Polly的弹性服务实现
public class ResilientService : IResilientService, IDisposable
{
    private readonly ResilientOptions _options;
    private readonly ILogger<ResilientService>? _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    private readonly AsyncTimeoutPolicy _timeoutPolicy;
    private readonly AsyncBulkheadPolicy _bulkheadPolicy;
    private readonly RateLimiter _rateLimiter;
    private readonly AsyncPolicyWrap _combinedPolicy;
    private readonly ConcurrentDictionary<string, ResilienceMetrics> _metrics = new();
    private Timer? _healthCheckTimer;
    private bool _disposed = false;

    public ResilientService(IOptions<ResilientOptions> options, ILogger<ResilientService>? logger = null)
    {
        _options = options.Value;
        _logger = logger;

        // 初始化限速器
        _rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = _options.RateLimiterPermitLimit,
            TokensPerPeriod = _options.RateLimiterPermitLimit,
            ReplenishmentPeriod = _options.RateLimiterWindow,
            QueueLimit = _options.RateLimiterQueueLimit
        });

        // 配置重试策略
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .Or<TransientHttpErrorException>()
            .WaitAndRetryAsync(
                _options.RetryCount,
                retryAttempt => _options.UseExponentialBackoff 
                    ? TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1)) 
                    : _options.RetryDelay,
                (exception, delay, retryCount, context) =>
                {
                    _logger?.LogWarning("[Retry] Attempt {RetryCount} failed. Waiting {Delay}ms before next retry. Exception: {Exception}",
                        retryCount, delay.TotalMilliseconds, exception.Message);
                    _metrics.GetOrAdd("retry", _ => new ResilienceMetrics()).RetriedOperations++;
                });

        // 配置断路器策略
        _circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .Or<TransientHttpErrorException>()
            .CircuitBreakerAsync(
                _options.CircuitBreakerFailureThreshold,
                _options.CircuitBreakerDurationOfBreak,
                onBreak: (exception, breakDuration) =>
                {
                    _logger?.LogWarning("[CircuitBreaker] Circuit broken for {BreakDuration}ms. Exception: {Exception}",
                        breakDuration.TotalMilliseconds, exception.Message);
                    _metrics.GetOrAdd("circuitbreaker", _ => new ResilienceMetrics()).CircuitBreakerOpens++;
                },
                onReset: () =>
                {
                    _logger?.LogInformation("[CircuitBreaker] Circuit reset");
                },
                onHalfOpen: () =>
                {
                    _logger?.LogInformation("[CircuitBreaker] Circuit half-open");
                });

        // 配置超时策略
        _timeoutPolicy = Policy
            .TimeoutAsync(_options.Timeout);

        // 配置舱壁隔离策略
        _bulkheadPolicy = Policy
            .BulkheadAsync(
                _options.MaxParallelization,
                _options.MaxQueuingActions);

        // 组合策略
        _combinedPolicy = Policy.WrapAsync(
            _retryPolicy,
            _circuitBreakerPolicy,
            _timeoutPolicy,
            _bulkheadPolicy);

        // 启动健康检查
        if (_options.EnableHealthCheck)
        {
            StartHealthCheck();
        }
    }

    // 启动健康检查
    private void StartHealthCheck()
    {
        _healthCheckTimer = new Timer(async _ =>
        {
            try
            {
                var result = await HealthCheckAsync();
                _logger?.LogInformation("[HealthCheck] Status: {Status}, Message: {Message}",
                    result.Status, result.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "[HealthCheck] Failed to perform health check");
            }
        }, null, _options.HealthCheckInterval, _options.HealthCheckInterval);
    }

    // 使用重试策略执行操作
    public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithPolicyAsync(_retryPolicy, operation, cancellationToken);
    }

    // 使用断路器模式执行操作
    public async Task<T> ExecuteWithCircuitBreakerAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithPolicyAsync(_circuitBreakerPolicy, operation, cancellationToken);
    }

    // 使用限速器执行操作
    public async Task<T> ExecuteWithRateLimiterAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        using var lease = await _rateLimiter.AcquireAsync(1, cancellationToken);
        if (lease.IsAcquired)
        {
            return await operation();
        }
        else
        {
            _metrics.GetOrAdd("ratelimiter", _ => new ResilienceMetrics()).RateLimitedOperations++;
            throw new InvalidOperationException("Rate limit exceeded");
        }
    }

    // 使用超时策略执行操作
    public async Task<T> ExecuteWithTimeoutAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithPolicyAsync(_timeoutPolicy, operation, cancellationToken);
    }

    // 使用回退策略执行操作
    public async Task<T> ExecuteWithFallbackAsync<T>(Func<Task<T>> operation, Func<Task<T>> fallback, CancellationToken cancellationToken = default)
    {
        var fallbackPolicy = Policy
            .Handle<Exception>()
            .FallbackAsync(async (context, cancellationToken) =>
            {
                _logger?.LogWarning("[Fallback] Operation failed, using fallback");
                return await fallback();
            });

        return await ExecuteWithPolicyAsync(fallbackPolicy, operation, cancellationToken);
    }

    // 使用舱壁隔离执行操作
    public async Task<T> ExecuteWithBulkheadAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithPolicyAsync(_bulkheadPolicy, operation, cancellationToken);
    }

    // 使用组合策略执行操作
    public async Task<T> ExecuteWithResilienceAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithPolicyAsync(_combinedPolicy, operation, cancellationToken);
    }

    // 通用策略执行方法
    private async Task<T> ExecuteWithPolicyAsync<T>(IAsyncPolicy policy, Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        var metrics = _metrics.GetOrAdd("general", _ => new ResilienceMetrics());
        metrics.TotalOperations++;

        try
        {
            return await policy.ExecuteAsync(ct => operation(), cancellationToken);
        }
        catch (Exception ex)
        {
            metrics.FailedOperations++;
            _logger?.LogError(ex, "[ResilientService] Operation failed");
            throw;
        }
    }

    // 获取健康状态
    public Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        var result = new HealthCheckResult
        {
            IsHealthy = true,
            Status = "Healthy",
            Message = "Resilient service is healthy"
        };

        // 收集指标
        foreach (var (key, metrics) in _metrics)
        {
            result.Metrics.Add(key, new
            {
                TotalOperations = metrics.TotalOperations,
                FailedOperations = metrics.FailedOperations,
                FailureRate = metrics.FailureRate,
                RetriedOperations = metrics.RetriedOperations,
                CircuitBreakerOpens = metrics.CircuitBreakerOpens,
                RateLimitedOperations = metrics.RateLimitedOperations,
                TimedOutOperations = metrics.TimedOutOperations
            });
        }

        return Task.FromResult(result);
    }

    // 释放资源
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // 释放资源
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _healthCheckTimer?.Dispose();
                _rateLimiter?.Dispose();
            }

            _disposed = true;
        }
    }
}

// 依赖注入扩展
public static class ResilientServiceExtensions
{
    public static IServiceCollection AddResilientServices(this IServiceCollection services)
    {
        return services.AddResilientServices(options => { });
    }

    public static IServiceCollection AddResilientServices(this IServiceCollection services, Action<ResilientOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IResilientService, ResilientService>();
        return services;
    }
}

// 临时HTTP错误异常
public class TransientHttpErrorException : Exception
{
    public TransientHttpErrorException(string message) : base(message)
    {}

    public TransientHttpErrorException(string message, Exception innerException) : base(message, innerException)
    {}
}

// 主程序
public class Program
{
    public static async Task Main(string[] args)
    {
        // 构建服务容器
        var services = new ServiceCollection();
        
        // 配置日志
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });
        
        // 注册弹性服务
        services.AddResilientServices(options =>
        {
            options.RetryCount = 3;
            options.RetryDelay = TimeSpan.FromSeconds(1);
            options.UseExponentialBackoff = true;
            options.CircuitBreakerFailureThreshold = 0.5;
            options.CircuitBreakerDurationOfBreak = TimeSpan.FromSeconds(30);
            options.Timeout = TimeSpan.FromSeconds(30);
            options.RateLimiterPermitLimit = 100;
            options.RateLimiterWindow = TimeSpan.FromSeconds(1);
            options.RateLimiterQueueLimit = 50;
            options.MaxParallelization = 100;
            options.MaxQueuingActions = 50;
            options.EnableHealthCheck = true;
            options.HealthCheckInterval = TimeSpan.FromMinutes(1);
        });
        
        // 注册HTTP客户端
        services.AddHttpClient();
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取弹性服务
        var resilientService = serviceProvider.GetRequiredService<IResilientService>();
        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        
        Console.WriteLine("Resilient Service Demo");
        Console.WriteLine("=" + new string('=', 50));
        
        try
        {
            // 示例1: 使用组合弹性策略调用API
            Console.WriteLine("\n1. Testing combined resilience strategy...");
            var weatherResult = await resilientService.ExecuteWithResilienceAsync(async () =>
            {
                return await httpClient.GetFromJsonAsync<WeatherForecast>("https://api.example.com/weather");
            });
            Console.WriteLine("Weather API call successful!");
            
            // 示例2: 使用重试策略
            Console.WriteLine("\n2. Testing retry strategy...");
            var retryResult = await resilientService.ExecuteWithRetryAsync(async () =>
            {
                // 模拟可能失败的操作
                if (new Random().Next(0, 3) > 0)
                {
                    throw new HttpRequestException("Simulated network error");
                }
                return "Success!";
            });
            Console.WriteLine($"Retry strategy result: {retryResult}");
            
            // 示例3: 使用限速器
            Console.WriteLine("\n3. Testing rate limiter...");
            var rateLimitTasks = Enumerable.Range(0, 10).Select(async i =>
            {
                try
                {
                    var result = await resilientService.ExecuteWithRateLimiterAsync(async () =>
                    {
                        await Task.Delay(100); // 模拟操作
                        return $"Request {i} completed";
                    });
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request {i} failed: {ex.Message}");
                }
            });
            await Task.WhenAll(rateLimitTasks);
            
            // 示例4: 健康检查
            Console.WriteLine("\n4. Testing health check...");
            var healthResult = await resilientService.HealthCheckAsync();
            Console.WriteLine($"Health check status: {healthResult.Status}");
            Console.WriteLine($"Is healthy: {healthResult.IsHealthy}");
            Console.WriteLine("Metrics:");
            foreach (var (key, value) in healthResult.Metrics)
            {
                Console.WriteLine($"  {key}: {value}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // 释放资源
            if (serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}

// 天气预测模型
public class WeatherForecast
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC * 1.8);
    public string? Summary { get; set; }
}