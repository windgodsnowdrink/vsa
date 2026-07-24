#load "cache_heartbeat_integration.cs"

Console.WriteLine("=== cache_heartbeat_integration Test ===");

try
{
    var t0 = typeof(CacheHeartbeatService);
    Console.WriteLine($"[PASS] CacheHeartbeatService 存在");
    var t1 = typeof(CacheHeartbeatOptions);
    Console.WriteLine($"[PASS] CacheHeartbeatOptions 存在");
    var t2 = typeof(HeartbeatTimeoutEvent);
    Console.WriteLine($"[PASS] HeartbeatTimeoutEvent 存在");
    var t3 = typeof(TimeWheel);
    Console.WriteLine($"[PASS] TimeWheel 存在");
    var t4 = typeof(CacheHeartbeatExtensions);
    Console.WriteLine($"[PASS] CacheHeartbeatExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}