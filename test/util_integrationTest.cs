#load "util_integration.cs"

Console.WriteLine("=== util_integration Test ===");

try
{
    var t0 = typeof(UtilIntegration.UtilOptions);
    Console.WriteLine($"[PASS] UtilOptions 存在");
    var t1 = typeof(UtilIntegration.UtilService);
    Console.WriteLine($"[PASS] UtilService 存在");
    var t2 = typeof(UtilIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(UtilIntegration.ExampleUsage);
    Console.WriteLine($"[PASS] ExampleUsage 存在");
    var t4 = typeof(UtilIntegration.IUtilService);
    Console.WriteLine($"[PASS] IUtilService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}