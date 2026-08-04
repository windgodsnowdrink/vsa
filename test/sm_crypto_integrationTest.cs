#load "sm_crypto_integration.cs"

Console.WriteLine("=== sm_crypto_integration Test ===");

try
{
    var t0 = typeof(SmCryptoIntegration.SmCryptoOptions);
    Console.WriteLine($"[PASS] SmCryptoOptions 存在");
    var t1 = typeof(SmCryptoIntegration.SmCryptoService);
    Console.WriteLine($"[PASS] SmCryptoService 存在");
    var t2 = typeof(SmCryptoIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(SmCryptoIntegration.ISmCryptoService);
    Console.WriteLine($"[PASS] ISmCryptoService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}