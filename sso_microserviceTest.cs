#load "sso_microservice.cs"

Console.WriteLine("=== sso_microservice Test ===");

try
{
    var t0 = typeof(SsoMicroservice);
    Console.WriteLine($"[PASS] SsoMicroservice 存在");
    var t1 = typeof(SsoBusConfig);
    Console.WriteLine($"[PASS] SsoBusConfig 存在");
    var t2 = typeof(SsoCommand);
    Console.WriteLine($"[PASS] SsoCommand record 存在");
    var t3 = typeof(SsoEvent);
    Console.WriteLine($"[PASS] SsoEvent record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}