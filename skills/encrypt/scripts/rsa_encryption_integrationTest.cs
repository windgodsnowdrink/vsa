#load "rsa_encryption_integration.cs"

Console.WriteLine("=== rsa_encryption_integration Test ===");

try
{
    var t0 = typeof(Vsa.Security.Cryptography.RsaEncryptionOptions);
    Console.WriteLine($"[PASS] RsaEncryptionOptions 存在");
    var t1 = typeof(Vsa.Security.Cryptography.RsaEncryptor);
    Console.WriteLine($"[PASS] RsaEncryptor 存在");
    var t2 = typeof(Vsa.Security.Cryptography.RsaEncryptionExtensions);
    Console.WriteLine($"[PASS] RsaEncryptionExtensions 存在");
    var t3 = typeof(Vsa.Security.Cryptography.IRsaEncryptor);
    Console.WriteLine($"[PASS] IRsaEncryptor 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}