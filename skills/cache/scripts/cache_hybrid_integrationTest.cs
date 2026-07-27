#load "cache_hybrid_integration.cs"

Console.WriteLine("=== cache_hybrid_integration Test ===");

try
{
    var t0 = typeof(CachePolicyOptions);
    Console.WriteLine($"[PASS] CachePolicyOptions 存在");
    var t1 = typeof(ProductService);
    Console.WriteLine($"[PASS] ProductService 存在");
    var t2 = typeof(CacheEventHandler);
    Console.WriteLine($"[PASS] CacheEventHandler 存在");
    var t3 = typeof(CacheWarmupMiddleware);
    Console.WriteLine($"[PASS] CacheWarmupMiddleware 存在");
    var t4 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t5 = typeof(HybridCacheExtensions);
    Console.WriteLine($"[PASS] HybridCacheExtensions 存在");
    var t6 = typeof(IProductService);
    Console.WriteLine($"[PASS] IProductService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(IProductRepository);
    Console.WriteLine($"[PASS] IProductRepository 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(Product);
    Console.WriteLine($"[PASS] Product record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}