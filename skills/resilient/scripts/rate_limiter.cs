#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package System.Threading.RateLimiting@8.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
#:package System.Buffers@4.5.1
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property PublishAot=true
#:property TrimMode=partial
#:property EnableCompressionInSingleFile=true
#:property SelfContained=true

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

// 限速器选项
public class RateLimiterOptions
{
    // 令牌桶限速器配置
    public int TokenLimit { get; set; } = 100;
    public int TokensPerPeriod { get; set; } = 100;
    public TimeSpan ReplenishmentPeriod { get; set; } = TimeSpan.FromSeconds(1);
    public int QueueLimit { get; set; } = 50;
    public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;
    
    // 滑动窗口限速器配置
    public int WindowLimit { get; set; } = 100;
    public TimeSpan Window { get; set; } = TimeSpan.FromSeconds(1);
    public bool AutoReplenishment { get; set; } = true;
    
    // 并发限速器配置
    public int PermitLimit { get; set; } = 100;
    
    // 健康检查配置
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);
    public bool EnableHealthCheck { get; set; } = true;
    
    // 遥测配置
    public bool EnableTelemetry { get; set; } = true;
}

// 限速器类型
public enum RateLimiterType
{
    TokenBucket,
    SlidingWindow,
    Concurrency
}

// 限速器服务接口
public interface IRateLimiterService
{
    // 获取限速器实例
    RateLimiter GetRateLimiter(string name = "default");
    
    // 执行受限操作
    Task<T> ExecuteWithRateLimiterAsync<T>(Func<Task<T>> operation, string limiterName = "default", CancellationToken cancellationToken = default);
    
    // 尝试获取令牌
    ValueTask<RateLimitLease> TryAcquireAsync(int permitCount = 1, string limiterName = "default", CancellationToken cancellationToken = default);
    
    // 获取限速器指标
    RateLimiterMetrics GetMetrics(string limiterName = "default");
    
    // 获取所有限速器指标
    Dictionary<string, RateLimiterMetrics> GetAllMetrics();
    
    // 重置限速器
    void ResetRateLimiter(string limiterName = "default");
    
    // 获取健康状态
    Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default);
}

// 限速器指标
public class RateLimiterMetrics
{
    public int TotalRequests { get; set; }
    public int GrantedRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int QueueLength { get; set; }
    public double RejectionRate => TotalRequests > 0 ? (double)RejectedRequests / TotalRequests : 0;
    public double GrantRate => TotalRequests > 0 ? (double)GrantedRequests / TotalRequests : 0;
}

// 健康检查结果
public class HealthCheckResult
{
    public bool IsHealthy { get; set; }
    public string Status { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, object> Metrics { get; set; } = new();
}

// 限速器服务实现
public class RateLimiterService : IRateLimiterService, IDisposable
{
    private readonly RateLimiterOptions _options;
    private readonly ILogger<RateLimiterService>? _logger;
    private readonly ConcurrentDictionary<string, RateLimiter> _rateLimiters = new();
    private readonly ConcurrentDictionary<string, RateLimiterMetrics> _metrics = new();
    private Timer? _healthCheckTimer;
    private bool _disposed = false;

    public RateLimiterService(IOptions<RateLimiterOptions> options, ILogger<RateLimiterService>? logger = null)
    {
        _options = options.Value;
        _logger = logger;

        // 初始化默认限速器
        _rateLimiters.GetOrAdd("default", _ => CreateRateLimiter(RateLimiterType.TokenBucket));

        // 启动健康检查
        if (_options.EnableHealthCheck)
        {
            StartHealthCheck();
        }
    }

    // 创建限速器
    private RateLimiter CreateRateLimiter(RateLimiterType type)
    {
        switch (type)
        {
            case RateLimiterType.TokenBucket:
                return new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
                {
                    TokenLimit = _options.TokenLimit,
                    TokensPerPeriod = _options.TokensPerPeriod,
                    ReplenishmentPeriod = _options.ReplenishmentPeriod,
                    QueueLimit = _options.QueueLimit,
                    QueueProcessingOrder = _options.QueueProcessingOrder
                });
            
            case RateLimiterType.SlidingWindow:
                return new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
                {
                    Window = _options.Window,
                    PermitLimit = _options.WindowLimit,
                    QueueLimit = _options.QueueLimit,
                    QueueProcessingOrder = _options.QueueProcessingOrder,
                    AutoReplenishment = _options.AutoReplenishment
                });
            
            case RateLimiterType.Concurrency:
                return new ConcurrencyLimiter(new ConcurrencyLimiterOptions
                {
                    PermitLimit = _options.PermitLimit,
                    QueueLimit = _options.QueueLimit,
                    QueueProcessingOrder = _options.QueueProcessingOrder
                });
            
            default:
                throw new ArgumentException("Invalid rate limiter type");
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
                _logger?.LogInformation("[RateLimiter] Health check status: {Status}, Message: {Message}",
                    result.Status, result.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "[RateLimiter] Failed to perform health check");
            }
        }, null, _options.HealthCheckInterval, _options.HealthCheckInterval);
    }

    // 获取限速器实例
    public RateLimiter GetRateLimiter(string name = "default")
    {
        return _rateLimiters.GetOrAdd(name, _ => CreateRateLimiter(RateLimiterType.TokenBucket));
    }

    // 执行受限操作
    public async Task<T> ExecuteWithRateLimiterAsync<T>(Func<Task<T>> operation, string limiterName = "default", CancellationToken cancellationToken = default)
    {
        var limiter = GetRateLimiter(limiterName);
        var metrics = _metrics.GetOrAdd(limiterName, _ => new RateLimiterMetrics());
        metrics.TotalRequests++;

        using var lease = await limiter.AcquireAsync(1, cancellationToken);
        if (lease.IsAcquired)
        {
            metrics.GrantedRequests++;
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "[RateLimiter] Operation failed");
                throw;
            }
        }
        else
        {
            metrics.RejectedRequests++;
            _logger?.LogWarning("[RateLimiter] Rate limit exceeded for {LimiterName}", limiterName);
            throw new InvalidOperationException($"Rate limit exceeded for {limiterName}");
        }
    }

    // 尝试获取令牌
    public async ValueTask<RateLimitLease> TryAcquireAsync(int permitCount = 1, string limiterName = "default", CancellationToken cancellationToken = default)
    {
        var limiter = GetRateLimiter(limiterName);
        var metrics = _metrics.GetOrAdd(limiterName, _ => new RateLimiterMetrics());
        metrics.TotalRequests++;

        var lease = await limiter.AcquireAsync(permitCount, cancellationToken);
        if (lease.IsAcquired)
        {
            metrics.GrantedRequests++;
        }
        else
        {
            metrics.RejectedRequests++;
            _logger?.LogWarning("[RateLimiter] Failed to acquire {PermitCount} permits for {LimiterName}", permitCount, limiterName);
        }
        
        return lease;
    }

    // 获取限速器指标
    public RateLimiterMetrics GetMetrics(string limiterName = "default")
    {
        return _metrics.GetOrAdd(limiterName, _ => new RateLimiterMetrics());
    }

    // 获取所有限速器指标
    public Dictionary<string, RateLimiterMetrics> GetAllMetrics()
    {
        return _metrics.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    // 重置限速器
    public void ResetRateLimiter(string limiterName = "default")
    {
        if (_rateLimiters.TryRemove(limiterName, out var limiter))
        {
            limiter.Dispose();
            _metrics.TryRemove(limiterName, out _);
            _logger?.LogInformation("[RateLimiter] Reset rate limiter: {LimiterName}", limiterName);
        }
    }

    // 获取健康状态
    public Task<HealthCheckResult> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        var result = new HealthCheckResult
        {
            IsHealthy = true,
            Status = "Healthy",
            Message = "Rate limiter service is healthy"
        };

        // 收集指标
        foreach (var (name, limiter) in _rateLimiters)
        {
            var metrics = GetMetrics(name);
            result.Metrics.Add(name, new
            {
                TotalRequests = metrics.TotalRequests,
                GrantedRequests = metrics.GrantedRequests,
                RejectedRequests = metrics.RejectedRequests,
                RejectionRate = metrics.RejectionRate,
                GrantRate = metrics.GrantRate
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
                foreach (var limiter in _rateLimiters.Values)
                {
                    limiter.Dispose();
                }
            }

            _disposed = true;
        }
    }
}

// 依赖注入扩展
public static class RateLimiterServiceExtensions
{
    public static IServiceCollection AddRateLimiterServices(this IServiceCollection services)
    {
        return services.AddRateLimiterServices(options => { });
    }

    public static IServiceCollection AddRateLimiterServices(this IServiceCollection services, Action<RateLimiterOptions> configureOptions)
    {
        services.Configure(configureOptions);
        services.AddSingleton<IRateLimiterService, RateLimiterService>();
        return services;
    }
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
        
        // 注册限速器服务
        services.AddRateLimiterServices(options =>
        {
            options.TokenLimit = 10;
            options.TokensPerPeriod = 10;
            options.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
            options.QueueLimit = 5;
            options.EnableHealthCheck = true;
            options.HealthCheckInterval = TimeSpan.FromMinutes(1);
        });
        
        var serviceProvider = services.BuildServiceProvider();
        
        // 获取限速器服务
        var rateLimiterService = serviceProvider.GetRequiredService<IRateLimiterService>();
        
        Console.WriteLine("Rate Limiter Service Demo");
        Console.WriteLine("=" + new string('=', 50));
        
        try
        {
            // 测试限速器
            Console.WriteLine("\nTesting rate limiter with 15 concurrent requests...");
            var tasks = Enumerable.Range(0, 15).Select(async i =>
            {
                try
                {
                    var result = await rateLimiterService.ExecuteWithRateLimiterAsync(async () =>
                    {
                        await Task.Delay(50); // 模拟操作
                        return $"Request {i} completed