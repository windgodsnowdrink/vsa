#load "oneremote_integration.cs"

Console.WriteLine("=== oneremote_integration Test ===");

try
{
    var t0 = typeof(OneRemoteIntegration.OneRemoteOptions);
    Console.WriteLine($"[PASS] OneRemoteOptions 存在");
    var t1 = typeof(OneRemoteIntegration.OneRemoteService);
    Console.WriteLine($"[PASS] OneRemoteService 存在");
    var t2 = typeof(OneRemoteIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(OneRemoteIntegration.ConnectionInfo);
    Console.WriteLine($"[PASS] ConnectionInfo 存在");
    var t4 = typeof(OneRemoteIntegration.SessionInfo);
    Console.WriteLine($"[PASS] SessionInfo 存在");
    var t5 = typeof(OneRemoteIntegration.ServerInfo);
    Console.WriteLine($"[PASS] ServerInfo 存在");
    var t6 = typeof(OneRemoteIntegration.ConnectionStats);
    Console.WriteLine($"[PASS] ConnectionStats 存在");
    var t7 = typeof(OneRemoteIntegration.ThroughputStats);
    Console.WriteLine($"[PASS] ThroughputStats 存在");
    var t8 = typeof(OneRemoteIntegration.HealthCheckResult);
    Console.WriteLine($"[PASS] HealthCheckResult 存在");
    var t9 = typeof(OneRemoteIntegration.IOneRemoteService);
    Console.WriteLine($"[PASS] IOneRemoteService 接口存在 (IsInterface: {t9.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}