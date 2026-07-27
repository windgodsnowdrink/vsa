#load "sha256_fingerprint_integration.cs"

Console.WriteLine("=== sha256_fingerprint_integration Test ===");

try
{
    var t0 = typeof(FingerprintIntegration.Sha256FingerprintOptions);
    Console.WriteLine($"[PASS] Sha256FingerprintOptions 存在");
    var t1 = typeof(FingerprintIntegration.Sha256FingerprintService);
    Console.WriteLine($"[PASS] Sha256FingerprintService 存在");
    var t2 = typeof(FingerprintIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(FingerprintIntegration.ISha256FingerprintService);
    Console.WriteLine($"[PASS] ISha256FingerprintService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}