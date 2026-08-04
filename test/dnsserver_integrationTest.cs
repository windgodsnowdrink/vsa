#load "dnsserver_integration.cs"

Console.WriteLine("=== dnsserver_integration Test ===");

try
{
    var t0 = typeof(DnsServerOptions);
    Console.WriteLine($"[PASS] DnsServerOptions 存在");
    var t1 = typeof(DnsServiceEntry);
    Console.WriteLine($"[PASS] DnsServiceEntry 存在");
    var t2 = typeof(DnsServerService);
    Console.WriteLine($"[PASS] DnsServerService 存在");
    var t3 = typeof(DnsServerExtensions);
    Console.WriteLine($"[PASS] DnsServerExtensions 存在");
    var t4 = typeof(IDnsServerService);
    Console.WriteLine($"[PASS] IDnsServerService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(var);
    Console.WriteLine($"[PASS] var record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}