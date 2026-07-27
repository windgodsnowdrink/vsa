#load "util_enhanced_integration.cs"

Console.WriteLine("=== util_enhanced_integration Test ===");

try
{
    var t0 = typeof(UtilEnhancedIntegration.UtilEnhancedOptions);
    Console.WriteLine($"[PASS] UtilEnhancedOptions 存在");
    var t1 = typeof(UtilEnhancedIntegration.UtilEnhancedService);
    Console.WriteLine($"[PASS] UtilEnhancedService 存在");
    var t2 = typeof(UtilEnhancedIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(UtilEnhancedIntegration.ExampleEnhancedUsage);
    Console.WriteLine($"[PASS] ExampleEnhancedUsage 存在");
    var t4 = typeof(UtilEnhancedIntegration.IUtilEnhancedService);
    Console.WriteLine($"[PASS] IUtilEnhancedService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}