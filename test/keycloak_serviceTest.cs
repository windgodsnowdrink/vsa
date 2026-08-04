#load "keycloak_service.cs"

Console.WriteLine("=== keycloak_service Test ===");

try
{
    var t0 = typeof(KeycloakService);
    Console.WriteLine($"[PASS] KeycloakService 存在");
    var t1 = typeof(KeycloakOptions);
    Console.WriteLine($"[PASS] KeycloakOptions 存在");
    var t2 = typeof(AuthRequest);
    Console.WriteLine($"[PASS] AuthRequest record 存在");
    var t3 = typeof(AuthResult);
    Console.WriteLine($"[PASS] AuthResult record 存在");
    var t4 = typeof(LoginRequest);
    Console.WriteLine($"[PASS] LoginRequest record 存在");
    var t5 = typeof(AuthStatus);
    Console.WriteLine($"[PASS] AuthStatus enum 存在 (IsEnum: {t5.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}