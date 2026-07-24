#:package BotSharp.Core@1.0.0
#:package Microsoft.Extensions.Options@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0

using BotSharp.Core;
using BotSharp.Core.Agents;
using BotSharp.Core.Conversations;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Caching.Memory;

public static class BotSharpServiceCollectionExtensions
{
    public static IServiceCollection AddBotSharpIntegration(
        this IServiceCollection services,
        Action<BotSharpOptions> configureOptions = null)
    {
        // 配置选项
        services.AddOptions<BotSharpOptions>()
            .Configure(options => configureOptions?.Invoke(options))
            .Validate(options => 
            {
                options.Validate();
                return true;
            });
            
        // 注册内存缓存
        services.TryAddSingleton<IMemoryCache, MemoryCache>();
        services.Configure<MemoryCacheOptions>(options => 
        {
            options.SizeLimit = 1024 * 1024 * 100; // 100MB默认限制
            options.CompactionPercentage = 0.2;
            options.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
        });
        
        // 注册监控
        services.TryAddSingleton<Meter>(sp => new Meter("BotSharp"));
        
        // 注册核心服务
        services.TryAddScoped<IBotSharpPipeline, BotSharpProcessor>();
        services.TryAddScoped<IBotSharpAgent, BotSharpAgent>();
        services.TryAddScoped<IConversationStorage, ConversationStorage>();
        
        // 配置OpenTelemetry导出器
        services.AddOpenTelemetry()
            .WithMetrics(metrics => 
            {
                metrics.AddMeter("BotSharp");
                metrics.AddOtlpExporter();
            });
            
        return services;
    }
}

// 配置类
public sealed record BotSharpOptions
{
    public string AgentId { get; set; } = string.Empty;
    public string ModelName { get; set; } = "gpt-3.5-turbo";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public bool EnableTracing { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
    public string OpenTelemetryEndpoint { get; set; } = "http://localhost:4317"; // OpenTelemetry Collector地址
    
    // 缓存配置
    public bool EnableResponseCache { get; set; } = true;
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public int CacheSizeLimit { get; set; } = 1000; // 缓存项数量限制
    public long CacheMemoryLimit { get; set; } = 1024 * 1024 * 100; // 100MB内存限制
    
    // 限流配置
    public bool EnableRateLimiting { get; set; } = true;
    public int RateLimitPerMinute { get; set; } = 60;
    public int BurstCapacity { get; set; } = 10; // 突发流量容量
    public TimeSpan WindowSize { get; set; } = TimeSpan.FromSeconds(1); // 时间窗口大小
    
    // 熔断配置
    public double CircuitBreakerFailureThreshold { get; set; } = 0.5; // 失败率阈值
    public int CircuitBreakerMinimumThroughput { get; set; } = 10; // 最小请求数
    public TimeSpan CircuitBreakerSamplingDuration { get; set; } = TimeSpan.FromSeconds(30); // 采样周期
    public TimeSpan CircuitBreakerDurationOfBreak { get; set; } = TimeSpan.FromSeconds(60); // 熔断持续时间
    
    // 验证方法
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AgentId))
            throw new ArgumentException("AgentId is required");
            
        if (string.IsNullOrWhiteSpace(ModelName))
            throw new ArgumentException("ModelName is required");
    }
}

// 服务接口
public interface IBotSharpPipeline : IAsyncDisposable
{
    Task<Conversation> StartConversationAsync(string userId, CancellationToken cancellationToken = default);
    Task<string> ProcessMessageAsync(string conversationId, string message, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> BatchProcessAsync(IEnumerable<string> messages, CancellationToken cancellationToken = default);
}

// 服务实现
public sealed class BotSharpProcessor : IBotSharpPipeline
{
    private readonly BotSharpOptions _options;
    private readonly ILogger<BotSharpProcessor> _logger;
    private readonly IBotSharpAgent _agent;
    private readonly IConversationStorage _conversationStorage;
    private readonly IMemoryCache _cache;
    private readonly AsyncRateLimiter _rateLimiter;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly Counter<int> _requestCounter;
    private readonly Histogram<double> _responseTimeHistogram;
    
    public BotSharpProcessor(
        IOptions<BotSharpOptions> options,
        ILogger<BotSharpProcessor> logger,
        IBotSharpAgent agent,
        IConversationStorage conversationStorage,
        IMemoryCache cache,
        Meter meter,
        IOptions<MemoryCacheOptions> cacheOptions = null)
    {
        _options = options.Value;
        _logger = logger;
        _agent = agent;
        _conversationStorage = conversationStorage;
        _cache = cache;
        
        // 配置缓存选项
        if (cacheOptions != null && _options.EnableResponseCache)
        {
            cacheOptions.Value.SizeLimit = _options.CacheMemoryLimit;
            cacheOptions.Value.CompactionPercentage = 0.2;
            cacheOptions.Value.ExpirationScanFrequency = TimeSpan.FromMinutes(1);
        }
        
        // 初始化限流器
        _rateLimiter = _options.EnableRateLimiting ? 
            new AsyncRateLimiter(
                _options.RateLimitPerMinute,
                _options.BurstCapacity,
                _options.WindowSize) : null;
            
        // 初始化熔断器
        _circuitBreaker = _options.EnableRateLimiting ?
            Policy.Handle<Exception>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5, // 50%失败率触发熔断
                    samplingDuration: TimeSpan.FromSeconds(30),
                    minimumThroughput: 10, // 最小请求数
                    durationOfBreak: TimeSpan.FromSeconds(60),
                    onBreak: (ex, state, duration) => 
                        _logger.LogWarning(ex, "Circuit breaker opened: {State} for {Duration}ms", state, duration.TotalMilliseconds),
                    onReset: () => _logger.LogInformation("Circuit breaker reset"),
                    onHalfOpen: () => _logger.LogInformation("Circuit breaker half-opened"))
            : null;
            
        // 初始化监控指标
        _requestCounter = meter.CreateCounter<int>("botsharp.requests", "count", "Total number of BotSharp requests");
        _responseTimeHistogram = meter.CreateHistogram<double>("botsharp.response_time", "milliseconds", "BotSharp request processing duration");
        
        // 添加更多监控维度
        var errorCounter = meter.CreateCounter<int>("botsharp.errors", "count", "Total number of BotSharp errors");
        var cacheHitCounter = meter.CreateCounter<int>("botsharp.cache_hits", "count", "Total number of cache hits");
        var rateLimitCounter = meter.CreateCounter<int>("botsharp.rate_limited", "count", "Total number of rate limited requests");
        
        // 配置OpenTelemetry导出器
        if (_options.EnableMetrics && !string.IsNullOrEmpty(_options.OpenTelemetryEndpoint))
        {
            var exporter = new OtlpExporter(new OtlpExporterOptions
            {
                Endpoint = new Uri(_options.OpenTelemetryEndpoint),
                Protocol = OtlpExportProtocol.HttpProtobuf
            });
            
            var reader = new PeriodicExportingMetricReader(exporter)
            {
                TemporalityPreference = MetricReaderTemporalityPreference.Delta
            };
            
            meter.AddReader(reader);
        }
    }
    
    public async Task<Conversation> StartConversationAsync(string userId, CancellationToken cancellationToken = default)
    {
        using var activity = Activity.Current?.Source.StartActivity("StartConversation");
        _requestCounter.Add(1, new TagList
            {
                { "operation", "StartConversation" },
                { "agent_id", _options.AgentId }
            });
            
            try
            {
            var stopwatch = Stopwatch.StartNew();
            
            // 限流检查
            if (_rateLimiter != null)
            {
                var waitResult = await _rateLimiter.WaitAsync(cancellationToken);
                if (!waitResult.Success)
                {
                    _logger.LogWarning("Rate limit exceeded for conversation {ConversationId}", conversationId);
                    throw new RateLimitException("Too many requests");
                }
            }
                
            var conversation = _circuitBreaker != null
                ? await _circuitBreaker.ExecuteAsync(() => 
                    _agent.StartConversationAsync(userId, cancellationToken))
                : await _agent.StartConversationAsync(userId, cancellationToken);
                
            await _conversationStorage.SaveAsync(conversation, cancellationToken);
            
            stopwatch.Stop();
            _responseTimeHistogram.Record(stopwatch.ElapsedMilliseconds);
            
            return conversation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting conversation for user {UserId}. ErrorType: {ErrorType}, Context: {Context}", 
                userId, ex.GetType().Name, new { ConversationId = conversationId, Message = message });
            
            // 根据错误类型决定是否重试
            if (ex is RateLimitException || ex is CircuitBreakerOpenException)
            {
                throw;
            }
            
            // 对于可重试错误，使用Polly重试策略
            var retryPolicy = Policy
                .Handle<Exception>(e => !(e is RateLimitException || e is CircuitBreakerOpenException))
                .WaitAndRetryAsync(
                    _options.MaxRetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, delay, retryCount, context) => 
                    {
                        _logger.LogWarning(exception, "Retry {RetryCount}/{MaxRetryCount} for conversation {ConversationId}. Waiting {Delay}ms",
                            retryCount, _options.MaxRetryCount, conversationId, delay.TotalMilliseconds);
                    });
            
            throw;
        }
    }
    
    public async Task<string> ProcessMessageAsync(string conversationId, string message, CancellationToken cancellationToken = default)
    {
        using var activity = Activity.Current?.Source.StartActivity("ProcessMessage");
        _requestCounter.Add(1, new TagList
            {
                { "operation", "ProcessMessage" },
                { "agent_id", _options.AgentId }
            });
            
            // 缓存键
        var cacheKey = $"response:{conversationId}:{message.GetHashCode()}";
        
        // 尝试从缓存获取
        if (_options.EnableResponseCache && _cache.TryGetValue<string>(cacheKey, out var cachedResponse))
        {
            _logger.LogDebug("Cache hit for message: {Message}", message);
            return cachedResponse;
        }
        
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            // 限流检查
            if (_rateLimiter != null)
                await _rateLimiter.WaitAsync(cancellationToken);
                
            var conversation = await _conversationStorage.GetAsync(conversationId, cancellationToken);
            if (conversation == null)
                throw new ArgumentException("Conversation not found", nameof(conversationId));
                
            string response = _circuitBreaker != null
                ? await _circuitBreaker.ExecuteAsync(() => 
                    _agent.ProcessAsync(conversation, message, cancellationToken))
                : await _agent.ProcessAsync(conversation, message, cancellationToken);
                
            // 存入缓存
            if (_options.EnableResponseCache)
            {
                _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _options.CacheExpiration,
                Size = Encoding.UTF8.GetByteCount(response),
                SizeLimit = _options.CacheMemoryLimit
            });
            }
            
            stopwatch.Stop();
            _responseTimeHistogram.Record(stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message for conversation {ConversationId}. ErrorType: {ErrorType}, Context: {Context}", 
                conversationId, ex.GetType().Name, new { Message = message });
            
            // 根据错误类型决定是否重试
            if (ex is RateLimitException || ex is CircuitBreakerOpenException)
            {
                throw;
            }
            
            // 对于可重试错误，使用Polly重试策略
            var retryPolicy = Policy
                .Handle<Exception>(e => !(e is RateLimitException || e is CircuitBreakerOpenException))
                .WaitAndRetryAsync(
                    _options.MaxRetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, delay, retryCount, context) => 
                    {
                        _logger.LogWarning(exception, "Retry {RetryCount}/{MaxRetryCount} for conversation {ConversationId}. Waiting {Delay}ms",
                            retryCount, _options.MaxRetryCount, conversationId, delay.TotalMilliseconds);
                    });
            
            throw;
        }
    }
    
    public async Task<IReadOnlyList<string>> BatchProcessAsync(IEnumerable<string> messages, CancellationToken cancellationToken = default)
    {
        using var activity = Activity.Current?.Source.StartActivity("BatchProcess");
        _requestCounter.Add(messages.Count());
        
        var stopwatch = Stopwatch.StartNew();
        var results = new List<string>();
        
        try
        {
            // 限流检查
            if (_rateLimiter != null)
                await _rateLimiter.WaitAsync(cancellationToken);
                
            foreach (var message in messages)
            {
                var response = _circuitBreaker != null
                    ? await _circuitBreaker.ExecuteAsync(() => 
                        ProcessMessageAsync(Guid.NewGuid().ToString(), message, cancellationToken))
                    : await ProcessMessageAsync(Guid.NewGuid().ToString(), message, cancellationToken);
                    
                results.Add(response);
            }
            
            stopwatch.Stop();
            _responseTimeHistogram.Record(stopwatch.ElapsedMilliseconds);
            
            return results.AsReadOnly();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error batch processing messages");
            throw;
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_rateLimiter != null)
            await _rateLimiter.DisposeAsync();
            
        GC.SuppressFinalize(this);
    }
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBotSharpIntegration(
        this IServiceCollection services,
        Action<BotSharpOptions> configureOptions)
    {
        services.AddOptions<BotSharpOptions>()
            .Configure(configureOptions)
            .Validate(options => 
            {
                options.Validate();
                return true;
            });
            
        services.AddMemoryCache();
        services.AddSingleton<IBotSharpPipeline, BotSharpProcessor>();
        
        // 添加监控
        services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddMeter("BotSharpProcessor")
                .AddPrometheusExporter());
                
        return services;
    }
}