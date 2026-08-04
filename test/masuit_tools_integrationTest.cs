#load "masuit_tools_integration.cs"

Console.WriteLine("=== masuit_tools_integration Test ===");

try
{
    var t0 = typeof(MasuitToolsIntegration.MasuitToolsOptions);
    Console.WriteLine($"[PASS] MasuitToolsOptions 存在");
    var t1 = typeof(MasuitToolsIntegration.MasuitToolsService);
    Console.WriteLine($"[PASS] MasuitToolsService 存在");
    var t2 = typeof(MasuitToolsIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(MasuitToolsIntegration.ExampleUsage);
    Console.WriteLine($"[PASS] ExampleUsage 存在");
    var t4 = typeof(MasuitToolsIntegration.IMasuitToolsService);
    Console.WriteLine($"[PASS] IMasuitToolsService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}