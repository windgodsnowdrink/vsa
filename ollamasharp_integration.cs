#:sdk Microsoft.NET.Sdk.Web
#:package OllamaSharp@1.0.0
#:package Microsoft.Extensions.Options@8.0.0
#:package Microsoft.Extensions.DependencyInjection@8.0.0
#:package OpenTelemetry.Extensions.Hosting@1.7.0
#:property LangVersion=preview
#:property TargetFramework=net10.0

using OllamaSharp;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Polly;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.MemoryPool;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

// 配置类
public class OllamaOptions
{
    public string ModelName { get; set; } = "llama2";
    public string Endpoint { get; set; } = "http://localhost:11434";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int BatchSize { get; set; } = 32;
    public bool EnableDistributedTracing { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public string ServiceName { get; set; } = "OllamaService";
    
    // 缓存配置
    public bool EnableResponseCache { get; set; } = true;
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public int CacheSizeLimit { get; set; } = 1000;
    
    // 限流配置
    public bool EnableRateLimiting { get; set; } = true;
    public int RateLimitPerMinute { get; set; } = 60;
    public int RateLimitBurst { get; set; } = 10;
    
    // 熔断配置
    public bool EnableCircuitBreaker { get; set; } = true;
    public int CircuitBreakerFailureThreshold { get; set; } = 5;
    public TimeSpan CircuitBreakerDuration { get; set; } = TimeSpan.FromSeconds(30);
    public double CircuitBreakerSamplingDuration { get; set; } = 0.5;
    
    // 新增模型管理配置
    public string ModelCachePath { get; set; } = "./models";
    public int MaxModelMemoryMB { get; set; } = 4096;
    
    // 提示工程配置
    public string DefaultSystemPrompt { get; set; } = "You are a helpful AI assistant.";
    public int MaxPromptLength { get; set; } = 4096;
    
    // 输出解析配置
    public bool EnableJsonOutput { get; set; } = false;
    public string OutputSchema { get; set; } = "";
    
    // 索引配置
    public string VectorIndexPath { get; set; } = "./index";
    public int VectorDimension { get; set; } = 768;
    
    // 内存管理
    public bool EnableMemoryPool { get; set; } = true;
    public int MemoryPoolSizeMB { get; set; } = 512;
    
    // 工具链配置
    public string[] Tools { get; set; } = Array.Empty<string>();
    public string AgentType { get; set; } = "ReAct";
}

// 服务接口
public interface IOllamaService
    {
        // 基础文本生成
        Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> StreamGenerateTextAsync(string prompt, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> BatchGenerateTextAsync(IEnumerable<string> prompts, CancellationToken cancellationToken = default);
        
        // 模型管理
        Task LoadModelAsync(string modelName, CancellationToken cancellationToken = default);
        Task UnloadModelAsync(CancellationToken cancellationToken = default);
        Task<string[]> ListAvailableModelsAsync(CancellationToken cancellationToken = default);
        
        // 提示工程
        Task<string> GenerateWithPromptTemplateAsync(string templateName, object variables, CancellationToken cancellationToken = default);
        Task RegisterPromptTemplateAsync(string name, string template, CancellationToken cancellationToken = default);
        
        // 输出解析
        Task<T> GenerateAndParseAsync<T>(string prompt, CancellationToken cancellationToken = default);
        Task<string> GenerateJsonAsync(string prompt, string schema, CancellationToken cancellationToken = default);
        
        // 索引与内存
        Task IndexTextAsync(string text, float[] embedding, CancellationToken cancellationToken = default);
        Task<string[]> SearchSimilarTextAsync(string query, int topK = 5, CancellationToken cancellationToken = default);
        Task<string> ChatWithMemoryAsync(string message, string conversationId, CancellationToken cancellationToken = default);
        
        // 工具与代理
        Task<string> UseToolAsync(string toolName, string input, CancellationToken cancellationToken = default);
        Task<string[]> ListAvailableToolsAsync(CancellationToken cancellationToken = default);
        Task RegisterToolAsync(string name, Func<string, Task<string>> handler, CancellationToken cancellationToken = default);
        Task<string> RunAgentAsync(string objective, CancellationToken cancellationToken = default);
        Task<string> RunPlannerAsync(string objective, string[] tools, CancellationToken cancellationToken = default);
        
        // 监控与配置
        void EnableTracing(ActivitySource activitySource);
        void EnableMetrics(Meter meter);
        void ConfigureRetryPolicy(IAsyncPolicy retryPolicy);
    }

// 服务实现
public class OllamaService : IOllamaService
{
    private readonly OllamaApiClient _client;
    private readonly ILogger<OllamaService> _logger;
    private ActivitySource _activitySource;
    private Meter _meter;
    private readonly MemoryPool<byte> _memoryPool;
    private readonly ConcurrentDictionary<string, string> _promptTemplates;
    private readonly ConcurrentDictionary<string, List<string>> _conversationMemories;
    private readonly ConcurrentDictionary<string, Func<string, Task<string>>> _tools;
    private IAsyncPolicy _retryPolicy;

    public OllamaService(
        OllamaApiClient client, 
        ILogger<OllamaService> logger,
        ActivitySource activitySource,
        Meter meter,
        MemoryPool<byte> memoryPool,
        IAsyncPolicy retryPolicy,
        IMemoryCache cache,
        IOptions<OllamaOptions> options)
    {
        _client = client;
        _logger = logger;
        _activitySource = activitySource;
        _meter = meter;
        _memoryPool = memoryPool;
        _promptTemplates = new ConcurrentDictionary<string, string>();
        _conversationMemories = new ConcurrentDictionary<string, List<string>>();
        _tools = new ConcurrentDictionary<string, Func<string, Task<string>>>();
        _retryPolicy = retryPolicy;
        _cache = cache;
        _options = options.Value;
        
        // 初始化限流器
        _rateLimiter = options.Value.EnableRateLimiting ? 
            new AsyncRateLimiter(options.Value.RateLimitPerMinute, options.Value.RateLimitBurst) : null;
            
        // 初始化熔断器
        _circuitBreaker = options.Value.EnableCircuitBreaker ?
            Policy.Handle<Exception>()
                .CircuitBreakerAsync(
                    options.Value.CircuitBreakerFailureThreshold,
                    options.Value.CircuitBreakerDuration,
                    (ex, state) => _logger.LogWarning("Circuit breaker opened: {State}", state),
                    () => _logger.LogInformation("Circuit breaker reset"))
            : null;
        
        // 注册默认工具
        RegisterDefaultTools();
    }
    
    private void RegisterDefaultTools()
    {
        _tools.TryAdd("calculator", async input => 
        {
            try {
                var expr = new System.Data.DataTable().Compute(input, null);
                return expr?.ToString() ?? "Calculation failed";
            } catch { return "Invalid expression"; }
        });
        
        _tools.TryAdd("web_search", async query => 
        {
            // 实际实现中应调用外部API
            return $"Search results for: {query}";
        });
    }

    public async Task<string> GenerateTextAsync(string prompt, CancellationToken cancellationToken = default)
    {
        using var activity = _activitySource.StartActivity("GenerateText");
        
        // 缓存键
        var cacheKey = $"response:{_options.ModelName}:{prompt.GetHashCode()}";
        
        // 尝试从缓存获取
        if (_options.EnableResponseCache && _cache.TryGetValue<string>(cacheKey, out var cachedResponse))
        {
            _logger.LogDebug("Cache hit for prompt: {Prompt}", prompt);
            return cachedResponse;
        }
        
        try
        {
            // 限流检查
            if (_rateLimiter != null)
            {
                await _rateLimiter.WaitAsync(cancellationToken);
            }
            
            // 熔断保护
            var response = _circuitBreaker != null 
                ? await _circuitBreaker.ExecuteAsync(() => 
                    _client.GenerateTextAsync(_options.ModelName, prompt, cancellationToken))
                : await _client.GenerateTextAsync(_options.ModelName, prompt, cancellationToken);
                
            // 存入缓存
            if (_options.EnableResponseCache)
            {
                _cache.Set(cacheKey, response.Response, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _options.CacheExpiration,
                    Size = 1 // 每个响应占1个单位
                });
            }
            
            return response.Response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating text for prompt: {Prompt}", prompt);
            throw;
        }
    }

    public async IAsyncEnumerable<string> StreamGenerateTextAsync(string prompt)
    {
        using var activity = _activitySource.StartActivity("StreamGenerateText");
        await foreach (var chunk in _client.StreamGenerateTextAsync(_options.ModelName, prompt))
        {
            yield return chunk.Response;
        }
    }

    public Task EnableTracingAsync(bool enabled) 
        => Task.FromResult(_options.EnableTracing = enabled);

    public Task ConfigureRetryPolicyAsync(int maxRetries, int retryDelayMs)
    {
        _options.MaxRetries = maxRetries;
        _options.RetryDelayMs = retryDelayMs;
        return Task.CompletedTask;
    }
}

// DI扩展方法
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOllama(this IServiceCollection services, Action<OllamaOptions> configureOptions)
    {
        services.AddOptions<OllamaOptions>().Configure(configureOptions);
        
        // 添加内存缓存
        services.AddMemoryCache(options => 
        {
            options.SizeLimit = 1024 * 1024; // 1MB
        });
        
        // 添加Polly策略
        services.AddSingleton<IAsyncPolicy>(sp => 
        {
            var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    options.MaxRetryCount, 
                    retryAttempt => options.RetryDelay);
        });
        
        services.AddSingleton<OllamaApiClient>(sp => 
        {
            var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
            return new OllamaApiClient(new Uri(options.Endpoint)) 
            { 
                Timeout = options.Timeout 
            };
        });
        
        services.AddSingleton<OllamaService>();
        
        // 添加监控
        services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics
                .AddMeter("OllamaService")
                .AddPrometheusExporter());
                
        return services;
        
        // 添加内存池
        services.AddSingleton<MemoryPool<byte>>(sp => 
            MemoryPool<byte>.Shared);
            
        services.AddSingleton<ActivitySource>(sp => 
            new ActivitySource(sp.GetRequiredService<IOptions<OllamaOptions>>().Value.ServiceName));
        
        services.AddSingleton<Meter>(sp => 
            new Meter(sp.GetRequiredService<IOptions<OllamaOptions>>().Value.ServiceName));
        
        services.AddSingleton<IAsyncPolicy>(sp => 
        {
            var options = sp.GetRequiredService<IOptions<OllamaOptions>>().Value;
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    options.MaxRetryCount, 
                    attempt => options.RetryDelay,
                    (ex, delay) => 
                        sp.GetRequiredService<ILogger<OllamaService>>()
                            .LogWarning(ex, "Retrying after delay: {Delay}", delay));
        });
        
        services.AddScoped<IOllamaService, OllamaService>();
        
        // 初始化模型缓存目录
        var options = new OllamaOptions();
        configureOptions(options);
        if (!Directory.Exists(options.ModelCachePath))
            Directory.CreateDirectory(options.ModelCachePath);
            
        // 初始化向量索引目录
        if (!Directory.Exists(options.VectorIndexPath))
            Directory.CreateDirectory(options.VectorIndexPath);
        
        return services;
    }
}
// 示例用法
var builder = WebApplication.CreateBuilder();
builder.Services.AddOllama(options =>
{
    options.ModelName = "llama2";
    options.Endpoint = "http://localhost:11434";
    options.Timeout = 300;
    options.EnableTracing = true;
    options.MaxRetries = 3;
});

var app = builder.Build();

app.MapGet("/generate", async (IOllamaService ollama, string prompt) => 
    await ollama.GenerateTextAsync(prompt));

app.MapGet("/stream", (IOllamaService ollama, string prompt) => 
    ollama.StreamGenerateTextAsync(prompt));

app.Run();
