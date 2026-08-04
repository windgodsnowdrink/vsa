#load "easycaching_demo.cs"

Console.WriteLine("=== easycaching_demo Test ===");

try
{
    var t0 = typeof(MemoryOptimizer);
    Console.WriteLine($"[PASS] MemoryOptimizer 存在");
    var t1 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t2 = typeof(TodoCacheService);
    Console.WriteLine($"[PASS] TodoCacheService 存在");
    var t3 = typeof(CacheWarmupService);
    Console.WriteLine($"[PASS] CacheWarmupService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}