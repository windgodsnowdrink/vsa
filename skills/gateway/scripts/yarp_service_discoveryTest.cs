#load "yarp_service_discovery.cs"

Console.WriteLine("=== yarp_service_discovery Test ===");

try
{
    var t0 = typeof(ServiceRegistryMiddleware);
    Console.WriteLine($"[PASS] ServiceRegistryMiddleware 存在");
    var t1 = typeof(DiscoveryClientPooledPolicy);
    Console.WriteLine($"[PASS] DiscoveryClientPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}