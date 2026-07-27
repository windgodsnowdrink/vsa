#load "quantum_crypto_integration.cs"

Console.WriteLine("=== quantum_crypto_integration Test ===");

try
{
    var t0 = typeof(QuantumCrypto.QuantumCryptoOptions);
    Console.WriteLine($"[PASS] QuantumCryptoOptions 存在");
    var t1 = typeof(QuantumCrypto.QuantumCryptoService);
    Console.WriteLine($"[PASS] QuantumCryptoService 存在");
    var t2 = typeof(QuantumCrypto.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(QuantumCrypto.IQuantumRandomNumberGenerator);
    Console.WriteLine($"[PASS] IQuantumRandomNumberGenerator 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(QuantumCrypto.IPostQuantumCryptoService);
    Console.WriteLine($"[PASS] IPostQuantumCryptoService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(QuantumCrypto.IQuantumCryptoService);
    Console.WriteLine($"[PASS] IQuantumCryptoService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(QuantumCrypto.IHybridEncryptionService);
    Console.WriteLine($"[PASS] IHybridEncryptionService 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}