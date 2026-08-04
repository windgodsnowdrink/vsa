#load "westwind_utilities_enhanced.cs"

Console.WriteLine("=== westwind_utilities_enhanced Test ===");

try
{
    var t0 = typeof(WestwindEnhancedOptions);
    Console.WriteLine($"[PASS] WestwindEnhancedOptions 存在");
    var t1 = typeof(WestwindEnhancedService);
    Console.WriteLine($"[PASS] WestwindEnhancedService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(IWestwindEnhancedService);
    Console.WriteLine($"[PASS] IWestwindEnhancedService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}