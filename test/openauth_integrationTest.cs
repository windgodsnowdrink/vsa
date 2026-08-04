#load "openauth_integration.cs"

Console.WriteLine("=== openauth_integration Test ===");

try
{
    var t0 = typeof(OpenAuthAdapter);
    Console.WriteLine($"[PASS] OpenAuthAdapter 存在");
    var t1 = typeof(OpenAuthConfig);
    Console.WriteLine($"[PASS] OpenAuthConfig 存在");
    var t2 = typeof(AuthRequest);
    Console.WriteLine($"[PASS] AuthRequest record 存在");
    var t3 = typeof(AuthResult);
    Console.WriteLine($"[PASS] AuthResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}