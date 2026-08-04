#load "redis_eventbus_integration.cs"

Console.WriteLine("=== redis_eventbus_integration Test ===");

try
{
    var t0 = typeof(RedisEventBus.Integration.RedisEventBusOptions);
    Console.WriteLine($"[PASS] RedisEventBusOptions 存在");
    var t1 = typeof(RedisEventBus.Integration.TieredMemoryServer);
    Console.WriteLine($"[PASS] TieredMemoryServer 存在");
    var t2 = typeof(RedisEventBus.Integration.RedisEventBus);
    Console.WriteLine($"[PASS] RedisEventBus 存在");
    var t3 = typeof(RedisEventBus.Integration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(RedisEventBus.Integration.ITieredMemoryServer);
    Console.WriteLine($"[PASS] ITieredMemoryServer 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(RedisEventBus.Integration.IRedisEventBus);
    Console.WriteLine($"[PASS] IRedisEventBus 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}