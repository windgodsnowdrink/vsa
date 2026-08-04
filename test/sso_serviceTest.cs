#load "sso_service.cs"

Console.WriteLine("=== sso_service Test ===");

try
{
    var t0 = typeof(SsoService);
    Console.WriteLine($"[PASS] SsoService 存在");
    var t1 = typeof(Config);
    Console.WriteLine($"[PASS] Config 存在");
    var t2 = typeof(SsoContext);
    Console.WriteLine($"[PASS] SsoContext 存在");
    var t3 = typeof(SsoContextPooledPolicy);
    Console.WriteLine($"[PASS] SsoContextPooledPolicy 存在");
    var t4 = typeof(SsoRequest);
    Console.WriteLine($"[PASS] SsoRequest record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}