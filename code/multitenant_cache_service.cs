#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package System.Threading.Channels@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable
#:property PublishAot true

using System.Buffers;
using System.Threading.Channels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.ObjectPool;
using System.Runtime.CompilerServices;

// 1. 租户缓存策略接口
public interface ITenantCachePolicy
{
    TimeSpan GetExpiration(string tenantId);
    long GetSizeLimit(string tenantId);
}

// 2. 高性能租户缓存服务(Disruptor模式)
[SkipLocalsInit]
public sealed class TenantAwareCacheService : BackgroundService
{
    private readonly Channel<CacheOperation> _operationChannel;
    private readonly ObjectPool<CacheContext> _contextPool;
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ITenantCachePolicy _cachePolicy;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    // 3. 使用ThreadLocal<Span>优化内存分配
    private readonly ThreadLocal<Span<byte>> _bufferCache = new(() => stackalloc byte[4096]);

    public TenantAwareCacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ITenantCachePolicy cachePolicy)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _cachePolicy = cachePolicy;
        _latencyOptimizer = new TailLatencyOptimizer();

        // 4. Disruptor模式通道配置
        _operationChannel = Channel.CreateBounded<CacheOperation>(new BoundedChannelOptions(10000)
        {
            SingleReader = true,
            AllowSynchronousContinuations = true,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        // 5. 上下文对象池(CPU cache-line对齐)
        _contextPool = new DefaultObjectPool<CacheContext>(
            new CacheContextPooledPolicy(), 
            Environment.ProcessorCount * 2);
    }

    // 6. 租户感知的缓存写入
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task SetAsync<T>(string tenantId, string key, T value)
    {
        var operation = new CacheOperation
        {
            OperationType = CacheOperationType.Set,
            TenantId = tenantId,
            Key = key,
            Value = value
        };

        await _operationChannel.Writer.WriteAsync(operation);
    }

    // 7. 租户感知的缓存读取(零拷贝优化)
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<T?> GetAsync<T>(string tenantId, string key)
    {
        var operation = new CacheOperation
        {
            OperationType = CacheOperationType.Get,
            TenantId = tenantId,
            Key = key
        };

        var resultChannel = Channel.CreateBounded<T?>(1);
        operation.ResultChannel = resultChannel;

        await _operationChannel.Writer.WriteAsync(operation);
        return await resultChannel.Reader.ReadAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var operation = await _operationChannel.Reader.ReadAsync(stoppingToken);
            var context = _contextPool.Get();

            try
            {
                switch (operation.OperationType)
                {
                    case CacheOperationType.Set:
                        await ProcessSetOperation(operation, context);
                        break;
                    case CacheOperationType.Get:
                        await ProcessGetOperation(operation, context);
                        break;
                }
            }
            finally
            {
                _contextPool.Return(context);
            }
        }
    }

    // 8. 处理缓存写入(使用Span优化)
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessSetOperation(CacheOperation operation, CacheContext context)
    {
        var tenantKey = $"{operation.TenantId}:{operation.Key}";
        var policy = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cachePolicy.GetExpiration(operation.TenantId),
            Size = _cachePolicy.GetSizeLimit(operation.TenantId)
        };

        _memoryCache.Set(tenantKey, operation.Value, policy);

        // 分布式缓存写入(使用零拷贝缓冲区)
        var buffer = _bufferCache.Value;
        if (operation.Value is byte[] bytes)
        {
            bytes.CopyTo(buffer);
            await _distributedCache.SetAsync(tenantKey, buffer[..bytes.Length]);
        }
        else
        {
            // 使用高性能序列化
            var serialized = MemoryPackSerializer.Serialize(operation.Value);
            serialized.CopyTo(buffer);
            await _distributedCache.SetAsync(tenantKey, buffer[..serialized.Length]);
        }
    }

    // 9. 处理缓存读取(使用Span优化)
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async Task ProcessGetOperation(CacheOperation operation, CacheContext context)
    {
        var tenantKey = $"{operation.TenantId}:{operation.Key}";

        // 首先检查内存缓存
        if (_memoryCache.TryGetValue(tenantKey, out var cachedValue) && 
            operation.ResultChannel != null)
        {
            await operation.ResultChannel.Writer.WriteAsync((T?)cachedValue);
            return;
        }

        // 检查分布式缓存(使用零拷贝缓冲区)
        var buffer = _bufferCache.Value;
        var distributedData = await _distributedCache.GetAsync(tenantKey);
        
        if (distributedData != null && operation.ResultChannel != null)
        {
            distributedData.CopyTo(buffer);
            var result = MemoryPackSerializer.Deserialize<T>(buffer[..distributedData.Length]);
            await operation.ResultChannel.Writer.WriteAsync(result);

            // 回填内存缓存
            var policy = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cachePolicy.GetExpiration(operation.TenantId),
                Size = _cachePolicy.GetSizeLimit(operation.TenantId)
            };
            _memoryCache.Set(tenantKey, result, policy);
        }
        else if (operation.ResultChannel != null)
        {
            await operation.ResultChannel.Writer.WriteAsync(default(T));
        }
    }
}

// 10. 缓存操作类型
public enum CacheOperationType
{
    Set,
    Get
}

// 11. 缓存操作结构(内存优化)
[SkipLocalsInit]
public struct CacheOperation
{
    public CacheOperationType OperationType { get; set; }
    public string TenantId { get; set; }
    public string Key { get; set; }
    public object? Value { get; set; }
    public Channel<object?>? ResultChannel { get; set; }
}

// 12. 缓存上下文(对象池优化)
[SkipLocalsInit]
public class CacheContext
{
    // 上下文特定资源
}

// 13. 缓存上下文池策略
public class CacheContextPooledPolicy : PooledObjectPolicy<CacheContext>
{
    public override CacheContext Create() => new();

    public override bool Return(CacheContext obj) => true;
}