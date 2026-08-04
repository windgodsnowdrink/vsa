#load "logging_dataprotection_extensions.cs"

Console.WriteLine("=== logging_dataprotection_extensions Test ===");

try
{
    var t0 = typeof(SensitiveDataProtectionExtensions);
    Console.WriteLine($"[PASS] SensitiveDataProtectionExtensions 存在");
    var t1 = typeof(ProtectedDataDestructuringPolicy);
    Console.WriteLine($"[PASS] ProtectedDataDestructuringPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}