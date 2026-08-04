#:sdk Microsoft.NET.Sdk.Web
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using System.Threading.RateLimiting;

// 分层速率限制器
public sealed class TieredRateLimiter : IDisposable
{
    private readonly RateLimiter _coreLimiter = new TokenBucketRateLimiter(new(
        tokenLimit: 1_000_000,
        queueProcessingOrder: QueueProcessingOrder.OldestFirst,
        tokensPerPeriod: 100_000,
        replenishmentPeriod: TimeSpan.FromMilliseconds(10)));
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async ValueTask<bool> TryProcessAsync(MessageEvent message)
    {
        using var lease = await _coreLimiter.AcquireAsync(1);
        return lease.IsAcquired;
    }
}

// 令牌桶限流
public sealed class TokenBucketLimiter : IDisposable
{
    private readonly RateLimiter _limiter;
    private readonly Timer _replenishTimer;
    
    public TokenBucketLimiter(int tokensPerSecond, int bucketCapacity)
    {
        _limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = bucketCapacity,
            TokensPerPeriod = tokensPerSecond,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1)
        });

        _replenishTimer = new Timer(_ => _limiter.TryReplenish(), 
            null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    public async ValueTask<bool> TryAcquireAsync()
    {
        using var lease = await _limiter.AcquireAsync(1);
        return lease.IsAcquired;
    }

    public void Dispose()
    {
        _limiter?.Dispose();
        _replenishTimer?.Dispose();
    }
}