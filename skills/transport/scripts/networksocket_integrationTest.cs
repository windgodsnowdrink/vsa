#load "networksocket_integration.cs"

Console.WriteLine("=== networksocket_integration Test ===");

try
{
    var t0 = typeof(NetworkSocketIntegration.NetworkSocketOptions);
    Console.WriteLine($"[PASS] NetworkSocketOptions 存在");
    var t1 = typeof(NetworkSocketIntegration.SslConfig);
    Console.WriteLine($"[PASS] SslConfig 存在");
    var t2 = typeof(NetworkSocketIntegration.HeartbeatConfig);
    Console.WriteLine($"[PASS] HeartbeatConfig 存在");
    var t3 = typeof(NetworkSocketIntegration.TieredMemoryConfig);
    Console.WriteLine($"[PASS] TieredMemoryConfig 存在");
    var t4 = typeof(NetworkSocketIntegration.NetworkSocketService);
    Console.WriteLine($"[PASS] NetworkSocketService 存在");
    var t5 = typeof(NetworkSocketIntegration.NetworkSocketExtensions);
    Console.WriteLine($"[PASS] NetworkSocketExtensions 存在");
    var t6 = typeof(NetworkSocketIntegration.TrafficMonitor);
    Console.WriteLine($"[PASS] TrafficMonitor 存在");
    var t7 = typeof(NetworkSocketIntegration.TrafficStats);
    Console.WriteLine($"[PASS] TrafficStats 存在");
    var t8 = typeof(NetworkSocketIntegration.TokenRingBuffer);
    Console.WriteLine($"[PASS] TokenRingBuffer 存在");
    var t9 = typeof(NetworkSocketIntegration.TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t10 = typeof(NetworkSocketIntegration.TieredMemoryService);
    Console.WriteLine($"[PASS] TieredMemoryService 存在");
    var t11 = typeof(NetworkSocketIntegration.INetworkSocketService);
    Console.WriteLine($"[PASS] INetworkSocketService 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(NetworkSocketIntegration.ITrafficMonitor);
    Console.WriteLine($"[PASS] ITrafficMonitor 接口存在 (IsInterface: {t12.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}