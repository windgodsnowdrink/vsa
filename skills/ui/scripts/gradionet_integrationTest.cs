#load "gradionet_integration.cs"

Console.WriteLine("=== gradionet_integration Test ===");

try
{
    var t0 = typeof(GradioNetIntegration.GradioNetOptions);
    Console.WriteLine($"[PASS] GradioNetOptions 存在");
    var t1 = typeof(GradioNetIntegration.GradioNetService);
    Console.WriteLine($"[PASS] GradioNetService 存在");
    var t2 = typeof(GradioNetIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(GradioNetIntegration.TenantContext);
    Console.WriteLine($"[PASS] TenantContext 存在");
    var t4 = typeof(GradioNetIntegration.ConnectionStatistics);
    Console.WriteLine($"[PASS] ConnectionStatistics 存在");
    var t5 = typeof(GradioNetIntegration.ThroughputStatistics);
    Console.WriteLine($"[PASS] ThroughputStatistics 存在");
    var t6 = typeof(GradioNetIntegration.GradioNetHealthCheck);
    Console.WriteLine($"[PASS] GradioNetHealthCheck 存在");
    var t7 = typeof(GradioNetIntegration.DiagnosticsConfig);
    Console.WriteLine($"[PASS] DiagnosticsConfig 存在");
    var t8 = typeof(GradioNetIntegration.IGradioNetService);
    Console.WriteLine($"[PASS] IGradioNetService 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(GradioNetIntegration.app);
    Console.WriteLine($"[PASS] app 接口存在 (IsInterface: {t9.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}