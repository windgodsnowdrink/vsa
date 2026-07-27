#load "sipsorcery_media_encryption_transport.cs"

Console.WriteLine("=== sipsorcery_media_encryption_transport Test ===");

try
{
    var t0 = typeof(MediaEncryptor);
    Console.WriteLine($"[PASS] MediaEncryptor 存在");
    var t1 = typeof(RTPProcessor);
    Console.WriteLine($"[PASS] RTPProcessor 存在");
    var t2 = typeof(QuantumSafeEncryptor);
    Console.WriteLine($"[PASS] QuantumSafeEncryptor 存在");
    var t3 = typeof(EncryptionConfig);
    Console.WriteLine($"[PASS] EncryptionConfig record 存在");
    var t4 = typeof(AesGcmConfig);
    Console.WriteLine($"[PASS] AesGcmConfig record 存在");
    var t5 = typeof(CompressionConfig);
    Console.WriteLine($"[PASS] CompressionConfig record 存在");
    var t6 = typeof(QuantumEncryptionConfig);
    Console.WriteLine($"[PASS] QuantumEncryptionConfig record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}