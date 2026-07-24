#load "ipaddressrange_integration.cs"

Console.WriteLine("=== ipaddressrange_integration Test ===");

try
{
    var t0 = typeof(IPAddressRangeIntegration.IpRangeOptions);
    Console.WriteLine($"[PASS] IpRangeOptions 存在");
    var t1 = typeof(IPAddressRangeIntegration.IPRangeInfo);
    Console.WriteLine($"[PASS] IPRangeInfo 存在");
    var t2 = typeof(IPAddressRangeIntegration.IpRangeService);
    Console.WriteLine($"[PASS] IpRangeService 存在");
    var t3 = typeof(IPAddressRangeIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(IPAddressRangeIntegration.IPAddressRangePooledObjectPolicy);
    Console.WriteLine($"[PASS] IPAddressRangePooledObjectPolicy 存在");
    var t5 = typeof(IPAddressRangeIntegration.IIpRangeService);
    Console.WriteLine($"[PASS] IIpRangeService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(IPAddressRangeIntegration.IpRangeCacheType);
    Console.WriteLine($"[PASS] IpRangeCacheType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}