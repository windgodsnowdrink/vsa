#load "cap_redis_integration.cs"

Console.WriteLine("=== cap_redis_integration Test ===");

try
{
    var t0 = typeof(ConsistentHashingShardingStrategy);
    Console.WriteLine($"[PASS] ConsistentHashingShardingStrategy 存在");
    var t1 = typeof(OptimizedMessageProcessor);
    Console.WriteLine($"[PASS] OptimizedMessageProcessor 存在");
    var t2 = typeof(MessageHandlerPoolPolicy);
    Console.WriteLine($"[PASS] MessageHandlerPoolPolicy 存在");
    var t3 = typeof(RedisShardingTransform);
    Console.WriteLine($"[PASS] RedisShardingTransform 存在");
    var t4 = typeof(ShardedConnectionPool);
    Console.WriteLine($"[PASS] ShardedConnectionPool 存在");
    var t5 = typeof(RedisDatabasePoolPolicy);
    Console.WriteLine($"[PASS] RedisDatabasePoolPolicy 存在");
    var t6 = typeof(MonitoringExtensions);
    Console.WriteLine($"[PASS] MonitoringExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}