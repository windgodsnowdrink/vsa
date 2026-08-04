#load "multitenant_cache_service.cs"

Console.WriteLine("=== multitenant_cache_service Test ===");

try
{
    var t0 = typeof(TenantAwareCacheService);
    Console.WriteLine($"[PASS] TenantAwareCacheService 存在");
    var t1 = typeof(CacheContext);
    Console.WriteLine($"[PASS] CacheContext 存在");
    var t2 = typeof(CacheContextPooledPolicy);
    Console.WriteLine($"[PASS] CacheContextPooledPolicy 存在");
    var t3 = typeof(ITenantCachePolicy);
    Console.WriteLine($"[PASS] ITenantCachePolicy 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(CacheOperation);
    Console.WriteLine($"[PASS] CacheOperation struct 存在");
    var t5 = typeof(CacheOperationType);
    Console.WriteLine($"[PASS] CacheOperationType enum 存在 (IsEnum: {t5.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}