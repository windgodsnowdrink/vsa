#load "service_discovery_advanced_integration.cs"

Console.WriteLine("=== service_discovery_advanced_integration Test ===");

try
{
    var t0 = typeof(SmartLoadBalancerFactory);
    Console.WriteLine($"[PASS] SmartLoadBalancerFactory 存在");
    var t1 = typeof(SmartLoadBalancer);
    Console.WriteLine($"[PASS] SmartLoadBalancer 存在");
    var t2 = typeof(ServiceDiscoveryHealthCheck);
    Console.WriteLine($"[PASS] ServiceDiscoveryHealthCheck 存在");
    var t3 = typeof(ServiceDiscoveryConfigurationChangeListener);
    Console.WriteLine($"[PASS] ServiceDiscoveryConfigurationChangeListener 存在");
    var t4 = typeof(ChangeTokenRegistration);
    Console.WriteLine($"[PASS] ChangeTokenRegistration 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}