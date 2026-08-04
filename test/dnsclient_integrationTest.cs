#load "dnsclient_integration.cs"

Console.WriteLine("=== dnsclient_integration Test ===");

try
{
    var t0 = typeof(DnsDiscoveryOptions);
    Console.WriteLine($"[PASS] DnsDiscoveryOptions 存在");
    var t1 = typeof(DnsServiceEntry);
    Console.WriteLine($"[PASS] DnsServiceEntry 存在");
    var t2 = typeof(DnsDiscoveryService);
    Console.WriteLine($"[PASS] DnsDiscoveryService 存在");
    var t3 = typeof(DnsDiscoveryExtensions);
    Console.WriteLine($"[PASS] DnsDiscoveryExtensions 存在");
    var t4 = typeof(IDnsDiscoveryService);
    Console.WriteLine($"[PASS] IDnsDiscoveryService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}