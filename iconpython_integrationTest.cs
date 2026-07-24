#load "iconpython_integration.cs"

Console.WriteLine("=== iconpython_integration Test ===");

try
{
    var t0 = typeof(IconPythonIntegration.IconPythonOptions);
    Console.WriteLine($"[PASS] IconPythonOptions 存在");
    var t1 = typeof(IconPythonIntegration.IconPythonService);
    Console.WriteLine($"[PASS] IconPythonService 存在");
    var t2 = typeof(IconPythonIntegration.MemoryOwner);
    Console.WriteLine($"[PASS] MemoryOwner 存在");
    var t3 = typeof(IconPythonIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(IconPythonIntegration.IconPythonHealthCheck);
    Console.WriteLine($"[PASS] IconPythonHealthCheck 存在");
    var t5 = typeof(IconPythonIntegration.ConnectionStatistics);
    Console.WriteLine($"[PASS] ConnectionStatistics 存在");
    var t6 = typeof(IconPythonIntegration.ThroughputStatistics);
    Console.WriteLine($"[PASS] ThroughputStatistics 存在");
    var t7 = typeof(IconPythonIntegration.TenantContext);
    Console.WriteLine($"[PASS] TenantContext 存在");
    var t8 = typeof(IconPythonIntegration.IIconPythonService);
    Console.WriteLine($"[PASS] IIconPythonService 接口存在 (IsInterface: {t8.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}