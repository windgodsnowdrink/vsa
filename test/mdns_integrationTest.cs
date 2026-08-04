#load "mdns_integration.cs"

Console.WriteLine("=== mdns_integration Test ===");

try
{
    var t0 = typeof(MdnsOptions);
    Console.WriteLine($"[PASS] MdnsOptions 存在");
    var t1 = typeof(MdnsService);
    Console.WriteLine($"[PASS] MdnsService 存在");
    var t2 = typeof(MdnsExtensions);
    Console.WriteLine($"[PASS] MdnsExtensions 存在");
    var t3 = typeof(IMdnsService);
    Console.WriteLine($"[PASS] IMdnsService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}