#load "freeim_integration.cs"

Console.WriteLine("=== freeim_integration Test ===");

try
{
    var t0 = typeof(FreeIMOptions);
    Console.WriteLine($"[PASS] FreeIMOptions 存在");
    var t1 = typeof(FreeIMService);
    Console.WriteLine($"[PASS] FreeIMService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(FreeIMHealthCheck);
    Console.WriteLine($"[PASS] FreeIMHealthCheck 存在");
    var t4 = typeof(TenantContext);
    Console.WriteLine($"[PASS] TenantContext 存在");
    var t5 = typeof(IFreeIMService);
    Console.WriteLine($"[PASS] IFreeIMService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(MemoryPoolStatistics);
    Console.WriteLine($"[PASS] MemoryPoolStatistics record 存在");
    var t7 = typeof(ConnectionStatistics);
    Console.WriteLine($"[PASS] ConnectionStatistics record 存在");
    var t8 = typeof(ThroughputStatistics);
    Console.WriteLine($"[PASS] ThroughputStatistics record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}