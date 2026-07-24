#load "ip2region_integration.cs"

Console.WriteLine("=== ip2region_integration Test ===");

try
{
    var t0 = typeof(Ip2RegionIntegration.Ip2RegionOptions);
    Console.WriteLine($"[PASS] Ip2RegionOptions 存在");
    var t1 = typeof(Ip2RegionIntegration.RegionInfo);
    Console.WriteLine($"[PASS] RegionInfo 存在");
    var t2 = typeof(Ip2RegionIntegration.Ip2RegionService);
    Console.WriteLine($"[PASS] Ip2RegionService 存在");
    var t3 = typeof(Ip2RegionIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(Ip2RegionIntegration.IIp2RegionService);
    Console.WriteLine($"[PASS] IIp2RegionService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(Ip2RegionIntegration.Ip2RegionCacheType);
    Console.WriteLine($"[PASS] Ip2RegionCacheType enum 存在 (IsEnum: {t5.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}