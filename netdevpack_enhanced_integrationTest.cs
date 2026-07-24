#load "netdevpack_enhanced_integration.cs"

Console.WriteLine("=== netdevpack_enhanced_integration Test ===");

try
{
    var t0 = typeof(NetDevPackIntegration.NetDevPackEnhancedOptions);
    Console.WriteLine($"[PASS] NetDevPackEnhancedOptions 存在");
    var t1 = typeof(NetDevPackIntegration.NetDevPackEnhancedService);
    Console.WriteLine($"[PASS] NetDevPackEnhancedService 存在");
    var t2 = typeof(NetDevPackIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(NetDevPackIntegration.ExampleEnhancedUsage);
    Console.WriteLine($"[PASS] ExampleEnhancedUsage 存在");
    var t4 = typeof(NetDevPackIntegration.INetDevPackEnhancedService);
    Console.WriteLine($"[PASS] INetDevPackEnhancedService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}