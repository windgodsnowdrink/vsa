#load "tye_integration.cs"

Console.WriteLine("=== tye_integration Test ===");

try
{
    var t0 = typeof(TyeIntegration.TyeOptions);
    Console.WriteLine($"[PASS] TyeOptions 存在");
    var t1 = typeof(TyeIntegration.TyeService);
    Console.WriteLine($"[PASS] TyeService 存在");
    var t2 = typeof(TyeIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(TyeIntegration.ITyeService);
    Console.WriteLine($"[PASS] ITyeService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}