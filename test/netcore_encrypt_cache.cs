#:sdk Microsoft.NET.Sdk.Web
#:package NETCore.Encrypt@3.0.0
#:package Microsoft.Extensions.Caching.StackExchangeRedis@8.0.0
#:package Microsoft.Extensions.Caching.Memory@8.0.0
#:package Polly@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable

using System.Collections;
using System.Runtime.CompilerServices;
using Polly;
using Polly.CircuitBreaker;

[SkipLocalsInit]
public sealed class EncryptCacheService : IDisposable
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ObjectPool<byte[]> _bufferPool;
    private readonly BitArray _bloomFilter;
    private readonly ConcurrentDictionary<string, long> _keyAccessCounts;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker;
    private readonly Timer _hotKeyDetector;
    
    // 1. BloomFilter初始化
    public EncryptCacheService(/* 依赖注入参数 */)
    {
        _bloomFilter = new BitArray(1_000_000); // 100万位布隆过滤器
        _keyAccessCounts = new ConcurrentDictionary<string, long>();
        
        // 2. 熔断器配置
        _circuitBreaker = Policy.Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));
        
        // 3. HotKey探测器
        _hotKeyDetector = new Timer(_ => 
        {
            foreach (var kv in _keyAccessCounts)
            {
                if (kv.Value > 100) // 阈值100次/分钟
                {
                    _memoryCache.Set($"hotkey_{kv.Key}", true, TimeSpan.FromMinutes(5));
                }
            }
            _keyAccessCounts.Clear();
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask<string> GetOrCreateEncryptedAsync(
        string key, 
        Func<string> valueFactory,
        TimeSpan? expiry = null)
    {
        // 1. BloomFilter检查
        if (!CheckBloomFilter(key))
            return default;
            
        // 2. HotKey检查
        if (_memoryCache.TryGetValue($"hotkey_{key}", out _))
        {
            Interlocked.Increment(ref _keyAccessCounts.GetOrAdd(key, 0));
            return await GetFromLocalCacheOnly(key);
        }
        
        // 3. CacheAside模式
        return await _circuitBreaker.ExecuteAsync(async () =>
        {
            if (_memoryCache.TryGetValue(key, out var cached))
                return cached;
                
            var buffer = _bufferPool.Get();
            try
            {
                var encrypted = await _distributedCache.GetStringAsync(key);
                if (encrypted != null)
                {
                    _memoryCache.Set(key, encrypted, expiry ?? TimeSpan.FromMinutes(5));
                    UpdateBloomFilter(key);
                    return encrypted;
                }

                var value = valueFactory();
                encrypted = EncryptProvider.AESEncrypt(value);
                
                await _distributedCache.SetStringAsync(key, encrypted, 
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expiry
                    });
                
                _memoryCache.Set(key, encrypted, expiry ?? TimeSpan.FromMinutes(5));
                UpdateBloomFilter(key);
                return encrypted;
            }
            finally
            {
                _bufferPool.Return(buffer);
            }
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private bool CheckBloomFilter(string key)
    {
        var hash = key.GetHashCode() % _bloomFilter.Length;
        return _bloomFilter[hash];
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void UpdateBloomFilter(string key)
    {
        var hash = key.GetHashCode() % _bloomFilter.Length;
        _bloomFilter[hash] = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private async ValueTask<string> GetFromLocalCacheOnly(string key)
    {
        if (_memoryCache.TryGetValue(key, out var cached))
            return cached;
            
        return await Task.FromResult<string>(null);
    }

    public void Dispose()
    {
        _hotKeyDetector?.Dispose();
    }
}