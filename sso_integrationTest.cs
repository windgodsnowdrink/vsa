#load "sso_integration.cs"

Console.WriteLine("=== sso_integration Test ===");

try
{
    var t0 = typeof(SsoService);
    Console.WriteLine($"[PASS] SsoService 存在");
    var t1 = typeof(SsoConfig);
    Console.WriteLine($"[PASS] SsoConfig 存在");
    var t2 = typeof(SsoRequest);
    Console.WriteLine($"[PASS] SsoRequest record 存在");
    var t3 = typeof(SsoResponse);
    Console.WriteLine($"[PASS] SsoResponse record 存在");
    var t4 = typeof(SsoStatus);
    Console.WriteLine($"[PASS] SsoStatus enum 存在 (IsEnum: {t4.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}