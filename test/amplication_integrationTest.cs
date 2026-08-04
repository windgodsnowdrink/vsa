#load "amplication_integration.cs"

Console.WriteLine("=== amplication_integration Test ===");

try
{
    var t0 = typeof(AmplicationIntegration);
    Console.WriteLine($"[PASS] AmplicationIntegration 存在");
    var t1 = typeof(AmplicationService);
    Console.WriteLine($"[PASS] AmplicationService 存在");
    var t2 = typeof(IAmplicationService);
    Console.WriteLine($"[PASS] IAmplicationService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}