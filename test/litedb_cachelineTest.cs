#load "litedb_cacheline.cs"

Console.WriteLine("=== litedb_cacheline Test ===");

try
{
    var t0 = typeof(CacheOptimizedRepository);
    Console.WriteLine($"[PASS] CacheOptimizedRepository 存在");
    var t1 = typeof(CacheLineAlignedEvent);
    Console.WriteLine($"[PASS] CacheLineAlignedEvent struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}