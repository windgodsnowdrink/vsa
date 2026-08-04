#load "longchain_integration.cs"

Console.WriteLine("=== longchain_integration Test ===");

try
{
    var t0 = typeof(LongChainOptions);
    Console.WriteLine($"[PASS] LongChainOptions 存在");
    var t1 = typeof(LongChainService);
    Console.WriteLine($"[PASS] LongChainService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(ILongChainService);
    Console.WriteLine($"[PASS] ILongChainService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}