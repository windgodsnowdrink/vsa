#load "autogen_integration.cs"

Console.WriteLine("=== autogen_integration Test ===");

try
{
    var t0 = typeof(AutoGenIntegration.AutoGenOptions);
    Console.WriteLine($"[PASS] AutoGenOptions 存在");
    var t1 = typeof(AutoGenIntegration.AutoGenService);
    Console.WriteLine($"[PASS] AutoGenService 存在");
    var t2 = typeof(AutoGenIntegration.AutoGenExtensions);
    Console.WriteLine($"[PASS] AutoGenExtensions 存在");
    var t3 = typeof(AutoGenIntegration.IAutoGenService);
    Console.WriteLine($"[PASS] IAutoGenService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}