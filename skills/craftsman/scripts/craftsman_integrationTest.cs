#load "craftsman_integration.cs"

Console.WriteLine("=== craftsman_integration Test ===");

try
{
    var t0 = typeof(CraftsmanIntegration);
    Console.WriteLine($"[PASS] CraftsmanIntegration 存在");
    var t1 = typeof(CraftsmanService);
    Console.WriteLine($"[PASS] CraftsmanService 存在");
    var t2 = typeof(ICraftsmanService);
    Console.WriteLine($"[PASS] ICraftsmanService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}