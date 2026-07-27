#load "garnet_redis_cache.cs"

Console.WriteLine("=== garnet_redis_cache Test ===");

try
{
    var t0 = typeof(Product);
    Console.WriteLine($"[PASS] Product 存在");
    var t1 = typeof(ProductDbContext);
    Console.WriteLine($"[PASS] ProductDbContext 存在");
    var t2 = typeof(ProductCacheService);
    Console.WriteLine($"[PASS] ProductCacheService 存在");
    var t3 = typeof(ProductController);
    Console.WriteLine($"[PASS] ProductController 存在");
    var t4 = typeof(CacheWarmupService);
    Console.WriteLine($"[PASS] CacheWarmupService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}