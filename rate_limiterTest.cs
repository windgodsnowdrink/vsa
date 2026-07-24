#load "rate_limiter.cs"

Console.WriteLine("=== rate_limiter Test ===");

try
{
    var t0 = typeof(TieredRateLimiter);
    Console.WriteLine($"[PASS] TieredRateLimiter 存在");
    var t1 = typeof(TokenBucketLimiter);
    Console.WriteLine($"[PASS] TokenBucketLimiter 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}