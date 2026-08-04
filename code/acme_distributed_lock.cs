#:sdk Microsoft.NET.Sdk.Web
#:package StackExchange.Redis@2.7.18
#:property LangVersion preview
#:property TargetFramework net10.0

using StackExchange.Redis;

[SkipLocalsInit]
public sealed class CertificateLockService
{
    private readonly IDatabase _redis;
    private readonly TailLatencyOptimizer _latencyOptimizer;

    public CertificateLockService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
        _latencyOptimizer = new TailLatencyOptimizer();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task<RedisLock> AcquireLockAsync(string domain, TimeSpan expiry)
    {
        using var latencyToken = _latencyOptimizer.BeginOperation();
        var token = Guid.NewGuid().ToString();
        var acquired = await _redis.LockTakeAsync(
            $"cert:lock:{domain}", 
            token, 
            expiry);
        
        return new RedisLock(_redis, domain, token, acquired);
    }
}

public readonly struct RedisLock : IDisposable
{
    private readonly IDatabase _db;
    private readonly string _key;
    private readonly string _token;

    public bool IsAcquired { get; }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Dispose()
    {
        if (IsAcquired)
        {
            _db.LockRelease(_key, _token);
        }
    }
}