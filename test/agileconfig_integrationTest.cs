#load "agileconfig_integration.cs"

Console.WriteLine("=== agileconfig_integration Test ===");

try
{
    var t0 = typeof(AgileConfigService);
    Console.WriteLine($"[PASS] AgileConfigService 存在");
    var t1 = typeof(AgileConfigExtensions);
    Console.WriteLine($"[PASS] AgileConfigExtensions 存在");
    var t2 = typeof(IAgileConfigService);
    Console.WriteLine($"[PASS] IAgileConfigService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}