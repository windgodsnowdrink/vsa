#load "westwind_utilities_integration.cs"

Console.WriteLine("=== westwind_utilities_integration Test ===");

try
{
    var t0 = typeof(WestwindOptions);
    Console.WriteLine($"[PASS] WestwindOptions 存在");
    var t1 = typeof(WestwindService);
    Console.WriteLine($"[PASS] WestwindService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(IWestwindService);
    Console.WriteLine($"[PASS] IWestwindService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}