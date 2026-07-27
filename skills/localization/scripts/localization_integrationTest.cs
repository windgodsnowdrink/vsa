#load "localization_integration.cs"

Console.WriteLine("=== localization_integration Test ===");

try
{
    var t0 = typeof(LocalizationIntegration.LocalizationService);
    Console.WriteLine($"[PASS] LocalizationService 存在");
    var t1 = typeof(LocalizationIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(LocalizationIntegration.JsonResourceWatcher);
    Console.WriteLine($"[PASS] JsonResourceWatcher 存在");
    var t3 = typeof(LocalizationIntegration.ILocalizationService);
    Console.WriteLine($"[PASS] ILocalizationService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}