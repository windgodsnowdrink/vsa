#load "mqtt_security.cs"

Console.WriteLine("=== mqtt_security Test ===");

try
{
    var t0 = typeof(AesGcmEncryptor);
    Console.WriteLine($"[PASS] AesGcmEncryptor 存在");
    var t1 = typeof(Sha256Fingerprinter);
    Console.WriteLine($"[PASS] Sha256Fingerprinter 存在");
    var t2 = typeof(AuditLogger);
    Console.WriteLine($"[PASS] AuditLogger 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}