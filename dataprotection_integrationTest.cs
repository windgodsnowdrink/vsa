#load "dataprotection_integration.cs"

Console.WriteLine("=== dataprotection_integration Test ===");

try
{
    var t0 = typeof(DataProtectionService);
    Console.WriteLine($"[PASS] DataProtectionService 存在");
    var t1 = typeof(ProtectorPooledPolicy);
    Console.WriteLine($"[PASS] ProtectorPooledPolicy 存在");
    var t2 = typeof(ProtectRequest);
    Console.WriteLine($"[PASS] ProtectRequest record 存在");
    var t3 = typeof(ProtectOperation);
    Console.WriteLine($"[PASS] ProtectOperation enum 存在 (IsEnum: {t3.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}