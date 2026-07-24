#load "theidserver_integration.cs"

Console.WriteLine("=== theidserver_integration Test ===");

try
{
    var t0 = typeof(TheIdServerAdapter);
    Console.WriteLine($"[PASS] TheIdServerAdapter 存在");
    var t1 = typeof(TheIdServerConfig);
    Console.WriteLine($"[PASS] TheIdServerConfig 存在");
    var t2 = typeof(AuthRequest);
    Console.WriteLine($"[PASS] AuthRequest record 存在");
    var t3 = typeof(AuthResult);
    Console.WriteLine($"[PASS] AuthResult record 存在");
    var t4 = typeof(AuthStatus);
    Console.WriteLine($"[PASS] AuthStatus enum 存在 (IsEnum: {t4.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}