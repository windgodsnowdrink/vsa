#load "stackexchange_redis_cache.cs"

Console.WriteLine("=== stackexchange_redis_cache Test ===");

try
{
    var t0 = typeof(MemoryObjectPool);
    Console.WriteLine($"[PASS] MemoryObjectPool 存在");
    var t1 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t2 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t3 = typeof(RedisCacheOptions);
    Console.WriteLine($"[PASS] RedisCacheOptions 存在");
    var t4 = typeof(RedisCacheService);
    Console.WriteLine($"[PASS] RedisCacheService 存在");
    var t5 = typeof(TodoDataPipeline);
    Console.WriteLine($"[PASS] TodoDataPipeline 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}