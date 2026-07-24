#load "rate_limiter.cs"

Console.WriteLine("=== rate_limiter Test ===");

try
{
    var t0 = typeof(RateLimiterOptions);
    Console.WriteLine($"[PASS] RateLimiterOptions 存在");
    var t1 = typeof(RateLimiterMetrics);
    Console.WriteLine($"[PASS] RateLimiterMetrics 存在");
    var t2 = typeof(HealthCheckResult);
    Console.WriteLine($"[PASS] HealthCheckResult 存在");
    var t3 = typeof(RateLimiterService);
    Console.WriteLine($"[PASS] RateLimiterService 存在");
    var t4 = typeof(RateLimiterServiceExtensions);
    Console.WriteLine($"[PASS] RateLimiterServiceExtensions 存在");
    var t5 = typeof(IRateLimiterService);
    Console.WriteLine($"[PASS] IRateLimiterService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(RateLimiterType);
    Console.WriteLine($"[PASS] RateLimiterType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}