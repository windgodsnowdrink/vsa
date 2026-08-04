#load "csgo_integration.cs"

Console.WriteLine("=== csgo_integration Test ===");

try
{
    var t0 = typeof(CsGoIntegration.CsGoOptions);
    Console.WriteLine($"[PASS] CsGoOptions 存在");
    var t1 = typeof(CsGoIntegration.CsGoService);
    Console.WriteLine($"[PASS] CsGoService 存在");
    var t2 = typeof(CsGoIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(CsGoIntegration.ICsGoService);
    Console.WriteLine($"[PASS] ICsGoService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}