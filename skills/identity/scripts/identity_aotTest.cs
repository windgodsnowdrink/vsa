#load "identity_aot.cs"

Console.WriteLine("=== identity_aot Test ===");

try
{
    var t0 = typeof(IdentitySettings);
    Console.WriteLine($"[PASS] IdentitySettings 存在");
    var t1 = typeof(TokenResult);
    Console.WriteLine($"[PASS] TokenResult 存在");
    var t2 = typeof(TokenValidationResult);
    Console.WriteLine($"[PASS] TokenValidationResult 存在");
    var t3 = typeof(TokenDecodeResult);
    Console.WriteLine($"[PASS] TokenDecodeResult 存在");
    var t4 = typeof(TokenRevocationResult);
    Console.WriteLine($"[PASS] TokenRevocationResult 存在");
    var t5 = typeof(TokenRefreshResult);
    Console.WriteLine($"[PASS] TokenRefreshResult 存在");
    var t6 = typeof(AuthenticationResult);
    Console.WriteLine($"[PASS] AuthenticationResult 存在");
    var t7 = typeof(AuthorizationResult);
    Console.WriteLine($"[PASS] AuthorizationResult 存在");
    var t8 = typeof(IdentityService);
    Console.WriteLine($"[PASS] IdentityService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}