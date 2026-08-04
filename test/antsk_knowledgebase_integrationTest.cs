#load "antsk_knowledgebase_integration.cs"

Console.WriteLine("=== antsk_knowledgebase_integration Test ===");

try
{
    var t0 = typeof(AntSKOptions);
    Console.WriteLine($"[PASS] AntSKOptions 存在");
    var t1 = typeof(AntSKService);
    Console.WriteLine($"[PASS] AntSKService 存在");
    var t2 = typeof(AntSKExtensions);
    Console.WriteLine($"[PASS] AntSKExtensions 存在");
    var t3 = typeof(AntSKBackgroundService);
    Console.WriteLine($"[PASS] AntSKBackgroundService 存在");
    var t4 = typeof(IAntSKService);
    Console.WriteLine($"[PASS] IAntSKService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}