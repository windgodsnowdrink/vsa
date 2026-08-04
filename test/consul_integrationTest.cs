#load "consul_integration.cs"

Console.WriteLine("=== consul_integration Test ===");

try
{
    var t0 = typeof(ConsulIntegration.ConsulOptions);
    Console.WriteLine($"[PASS] ConsulOptions 存在");
    var t1 = typeof(ConsulIntegration.ConsulService);
    Console.WriteLine($"[PASS] ConsulService 存在");
    var t2 = typeof(ConsulIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(ConsulIntegration.ConsulException);
    Console.WriteLine($"[PASS] ConsulException 存在");
    var t4 = typeof(ConsulIntegration.ConsulConnectionStats);
    Console.WriteLine($"[PASS] ConsulConnectionStats 存在");
    var t5 = typeof(ConsulIntegration.ConsulThroughputStats);
    Console.WriteLine($"[PASS] ConsulThroughputStats 存在");
    var t6 = typeof(ConsulIntegration.IConsulService);
    Console.WriteLine($"[PASS] IConsulService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(ConsulIntegration.ITenantContext);
    Console.WriteLine($"[PASS] ITenantContext 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}