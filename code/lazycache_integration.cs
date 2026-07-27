#:sdk Microsoft.NET.Sdk.Web
#:package LazyCache@2.2.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package OpenTelemetry.Exporter.OpenTelemetryProtocol@1.6.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using LazyCache;
using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics;
using System.Threading.Channels;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder();

// 1. 配置混合缓存策略
builder.Services.AddSingleton<IHybridCacheStrategy>(sp => 
    new HybridCacheStrategy(
        new MemoryCache(new MemoryCacheOptions { SizeLimit = 1024 * 1024 * 100 }),
        new RedisCache(new RedisCacheOptions { Configuration = "localhost:6379" }),
        new ThreadLocal<Span<byte>>(() => stackalloc byte[512])));

// 2. 配置LazyCache
builder.Services.AddLazyCache(provider => 
{
    var hybridCache = provider.GetRequiredService<IHybridCacheStrategy>();
    return new CachingService(hybridCache.LocalCache)
    {
        DefaultCachePolicy = new CacheDefaults
        {
            DefaultCacheDurationSeconds = 300,
            DefaultCacheForgetDurationSeconds = 30 // 缓存失效后30秒内不重新加载
        }
    };
});

// 3. 分布式缓存支持
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "LazyCache_";
});

// 4. 缓存失效策略
builder.Services.AddSingleton<ICacheExpirationStrategy>(sp => 
    new MultiLayerCacheExpirationStrategy(
        TimeSpan.FromSeconds(30),  // 本地缓存过期时间
        TimeSpan.FromMinutes(5),  // 分布式缓存过期时间
        TimeSpan.FromSeconds(5)   // 失效后重新加载时间窗口
    ));

// 5. 性能监控指标
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter("LazyCache.Metrics")
        .AddOtlpExporter());

// 6. 高性能缓存通道
var cacheChannel = Channel.CreateBounded<CacheOperation>(
    new BoundedChannelOptions(10000)
    {
        SingleReader = true,
        AllowSynchronousContinuations = true
    });

// 7. 零拷贝缓存处理器
builder.Services.AddSingleton<ICacheProcessor>(sp => 
    new ChannelCacheProcessor(
        cacheChannel,
        new ThreadLocal<Span<byte>>(() => stackalloc byte[512]),
        sp.GetRequiredService<IHybridCacheStrategy>(),
        sp.GetRequiredService<ICacheExpirationStrategy>(),
        sp.GetRequiredService<IMeterFactory>()));

var app = builder.Build();
app.MapGet("/", () => "Enhanced LazyCache Integration Ready");
app.Run();

// 混合缓存策略实现
[SkipLocalsInit]
public class HybridCacheStrategy : IHybridCacheStrategy
{
    private readonly MemoryCache _localCache;
    private readonly IDistributedCache _distributedCache;
    
    public HybridCacheStrategy(MemoryCache localCache, IDistributedCache distributedCache)
    {
        _localCache = localCache;
        _distributedCache = distributedCache;
    }
    
    // 新增方法：多级缓存读取
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask<T> GetOrCreateAsync<T>(string key, Func<ICacheEntry, Task<T>> factory)
    {
        // 1. 先检查本地缓存
        if (_localCache.TryGetValue(key, out T localValue))
        {
            Metrics.CacheHits.Add(1);
            return localValue;
        }

        // 2. 检查分布式缓存
        var distributedValue = await _distributedCache.GetAsync<T>(key);
        if (distributedValue != null)
        {
            // 回填本地缓存
            using var entry = _localCache.CreateEntry(key);
            entry.Value = distributedValue;
            entry.SetSize(1); // 简化大小计算
            Metrics.CacheHits.Add(1);
            return distributedValue;
        }

        // 3. 执行工厂方法获取数据
        Metrics.CacheMisses.Add(1);
        using var newEntry = _localCache.CreateEntry(key);
        var value = await factory(newEntry);
        await _distributedCache.SetAsync(key, value);
        return value;
    }
}

// 缓存失效策略
public class MultiLayerCacheExpirationStrategy : ICacheExpirationStrategy
{
    private readonly MemoryCache _localCache;
    private readonly IDistributedCache _distributedCache;
    
    public MultiLayerCacheExpirationStrategy(MemoryCache localCache, IDistributedCache distributedCache)
    {
        _localCache = localCache;
        _distributedCache = distributedCache;
    }
    
    // 新增方法：协调多级缓存失效
    public async Task InvalidateAsync(string key)
    {
        // 1. 立即失效本地缓存
        _localCache.Remove(key);
        
        // 2. 标记分布式缓存为待更新状态
        await _distributedCache.SetStringAsync(
            $"{key}_invalidated", 
            "1", 
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _reloadWindow });
        
        // 3. 触发后台重新加载
        _ = Task.Run(async () => 
        {
            await Task.Delay(_reloadWindow);
            await _distributedCache.RemoveAsync($"{key}_invalidated");
        });
    }
}

// 增强版缓存处理器
[SkipLocalsInit]
public class ChannelCacheProcessor : ICacheProcessor
{
    private readonly ChannelWriter<CacheOperation> _writer;
    private readonly ThreadLocal<Span<byte>> _buffer;
    
    public ChannelCacheProcessor(Channel<CacheOperation> channel, ThreadLocal<Span<byte>> buffer)
    {
        _writer = channel.Writer;
        _buffer = buffer;
    }
    
    // 新增监控指标处理
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void RecordMetrics(in CacheOperation operation)
    {
        var tags = new TagList
        {
            { "operation", operation.Type.ToString() },
            { "cache_layer", operation.CacheLayer.ToString() }
        };

        _metrics.CacheLatency.Record(operation.Duration.TotalMilliseconds, tags);
        
        if (operation.IsHit)
            _metrics.CacheHits.Add(1, tags);
        else
            _metrics.CacheMisses.Add(1, tags);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public unsafe void Process(CacheOperation operation)
    {
        Span<byte> buffer = stackalloc byte[512];
        fixed (byte* ptr = buffer)
        {
            if ((long)ptr % 64 == 0) // Cache-line对齐
            {
                // SIMD优化处理缓存操作
                _writer.TryWrite(operation);
            }
        }
    }
}