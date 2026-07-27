#load "audit_encryption.cs"

Console.WriteLine("=== audit_encryption Test ===");

try
{
    var t0 = typeof(AuditEncryptor);
    Console.WriteLine($"[PASS] AuditEncryptor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}