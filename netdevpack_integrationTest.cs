#load "netdevpack_integration.cs"

Console.WriteLine("=== netdevpack_integration Test ===");

try
{
    var t0 = typeof(NetDevPackIntegration.NetDevPackOptions);
    Console.WriteLine($"[PASS] NetDevPackOptions 存在");
    var t1 = typeof(NetDevPackIntegration.NetDevPackService);
    Console.WriteLine($"[PASS] NetDevPackService 存在");
    var t2 = typeof(NetDevPackIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(NetDevPackIntegration.ExampleUsage);
    Console.WriteLine($"[PASS] ExampleUsage 存在");
    var t4 = typeof(NetDevPackIntegration.UserRegisteredEvent);
    Console.WriteLine($"[PASS] UserRegisteredEvent 存在");
    var t5 = typeof(NetDevPackIntegration.INetDevPackService);
    Console.WriteLine($"[PASS] INetDevPackService 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}