#load "aspire_integration.cs"

Console.WriteLine("=== aspire_integration Test ===");

try
{
    var t0 = typeof(AspireIntegration.AspireOptions);
    Console.WriteLine($"[PASS] AspireOptions 存在");
    var t1 = typeof(AspireIntegration.AspireService);
    Console.WriteLine($"[PASS] AspireService 存在");
    var t2 = typeof(AspireIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(AspireIntegration.HealthReport);
    Console.WriteLine($"[PASS] HealthReport 存在");
    var t4 = typeof(AspireIntegration.MetricsSnapshot);
    Console.WriteLine($"[PASS] MetricsSnapshot 存在");
    var t5 = typeof(AspireIntegration.IAspireService);
    Console.WriteLine($"[PASS] IAspireService 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}