#load "redis_stream_integration.cs"

Console.WriteLine("=== redis_stream_integration Test ===");

try
{
    var t0 = typeof(RedisStreamIntegration.RedisStreamOptions);
    Console.WriteLine($"[PASS] RedisStreamOptions 存在");
    var t1 = typeof(RedisStreamIntegration.RedisStreamService);
    Console.WriteLine($"[PASS] RedisStreamService 存在");
    var t2 = typeof(RedisStreamIntegration.RedisStreamExtensions);
    Console.WriteLine($"[PASS] RedisStreamExtensions 存在");
    var t3 = typeof(RedisStreamIntegration.IRedisStreamService);
    Console.WriteLine($"[PASS] IRedisStreamService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}