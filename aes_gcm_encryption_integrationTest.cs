#load "aes_gcm_encryption_integration.cs"

Console.WriteLine("=== aes_gcm_encryption_integration Test ===");

try
{
    var t0 = typeof(AesGcmEncryption.AesGcmOptions);
    Console.WriteLine($"[PASS] AesGcmOptions 存在");
    var t1 = typeof(AesGcmEncryption.AesGcmEncryptor);
    Console.WriteLine($"[PASS] AesGcmEncryptor 存在");
    var t2 = typeof(AesGcmEncryption.AesGcmExtensions);
    Console.WriteLine($"[PASS] AesGcmExtensions 存在");
    var t3 = typeof(AesGcmEncryption.IAesGcmEncryptor);
    Console.WriteLine($"[PASS] IAesGcmEncryptor 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}