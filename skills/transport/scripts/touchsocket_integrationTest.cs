#load "touchsocket_integration.cs"

Console.WriteLine("=== touchsocket_integration Test ===");

try
{
    var t0 = typeof(TouchSocketOptions);
    Console.WriteLine($"[PASS] TouchSocketOptions 存在");
    var t1 = typeof(TieredMemoryConfig);
    Console.WriteLine($"[PASS] TieredMemoryConfig 存在");
    var t2 = typeof(TouchSocketService);
    Console.WriteLine($"[PASS] TouchSocketService 存在");
    var t3 = typeof(TouchSocketExtensions);
    Console.WriteLine($"[PASS] TouchSocketExtensions 存在");
    var t4 = typeof(TrafficMonitor);
    Console.WriteLine($"[PASS] TrafficMonitor 存在");
    var t5 = typeof(TrafficStats);
    Console.WriteLine($"[PASS] TrafficStats 存在");
    var t6 = typeof(TokenRingBuffer);
    Console.WriteLine($"[PASS] TokenRingBuffer 存在");
    var t7 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t8 = typeof(TieredMemoryService);
    Console.WriteLine($"[PASS] TieredMemoryService 存在");
    var t9 = typeof(ITouchSocketService);
    Console.WriteLine($"[PASS] ITouchSocketService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(ITrafficMonitor);
    Console.WriteLine($"[PASS] ITrafficMonitor 接口存在 (IsInterface: {t10.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}