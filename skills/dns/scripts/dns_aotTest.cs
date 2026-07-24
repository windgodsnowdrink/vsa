#load "dns_aot.cs"

Console.WriteLine("=== dns_aot Test ===");

try
{
    var t0 = typeof(Dns.AOT.DnsQueryResult);
    Console.WriteLine($"[PASS] DnsQueryResult 存在");
    var t1 = typeof(Dns.AOT.DnsOptions);
    Console.WriteLine($"[PASS] DnsOptions 存在");
    var t2 = typeof(Dns.AOT.DnsCacheItem);
    Console.WriteLine($"[PASS] DnsCacheItem 存在");
    var t3 = typeof(Dns.AOT.DnsStatus);
    Console.WriteLine($"[PASS] DnsStatus 存在");
    var t4 = typeof(Dns.AOT.DnsService);
    Console.WriteLine($"[PASS] DnsService 存在");
    var t5 = typeof(Dns.AOT.DnsAotEngine);
    Console.WriteLine($"[PASS] DnsAotEngine 存在");
    var t6 = typeof(Dns.AOT.DnsExtensions);
    Console.WriteLine($"[PASS] DnsExtensions 存在");
    var t7 = typeof(Dns.AOT.IDnsService);
    Console.WriteLine($"[PASS] IDnsService 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(Dns.AOT.example);
    Console.WriteLine($"[PASS] example record 存在");
    var t9 = typeof(Dns.AOT.DnsRecordType);
    Console.WriteLine($"[PASS] DnsRecordType enum 存在 (IsEnum: {t9.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}