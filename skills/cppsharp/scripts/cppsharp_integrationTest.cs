#load "cppsharp_integration.cs"

Console.WriteLine("=== cppsharp_integration Test ===");

try
{
    var t0 = typeof(CppSharpIntegration.CppSharpOptions);
    Console.WriteLine($"[PASS] CppSharpOptions 存在");
    var t1 = typeof(CppSharpIntegration.CppSharpService);
    Console.WriteLine($"[PASS] CppSharpService 存在");
    var t2 = typeof(CppSharpIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(CppSharpIntegration.TenantContext);
    Console.WriteLine($"[PASS] TenantContext 存在");
    var t4 = typeof(CppSharpIntegration.ConnectionStatistics);
    Console.WriteLine($"[PASS] ConnectionStatistics 存在");
    var t5 = typeof(CppSharpIntegration.ThroughputStatistics);
    Console.WriteLine($"[PASS] ThroughputStatistics 存在");
    var t6 = typeof(CppSharpIntegration.CppSharpHealthCheck);
    Console.WriteLine($"[PASS] CppSharpHealthCheck 存在");
    var t7 = typeof(CppSharpIntegration.MemoryPooledPolicy);
    Console.WriteLine($"[PASS] MemoryPooledPolicy 存在");
    var t8 = typeof(CppSharpIntegration.ICppSharpService);
    Console.WriteLine($"[PASS] ICppSharpService 接口存在 (IsInterface: {t8.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}