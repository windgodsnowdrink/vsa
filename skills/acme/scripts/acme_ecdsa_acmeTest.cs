#load "acme_ecdsa_acme.cs"

Console.WriteLine("=== acme_ecdsa_acme Test ===");

try
{
    var t0 = typeof(EcdsaCertificateService);
    Console.WriteLine($"[PASS] EcdsaCertificateService 存在");
    var t1 = typeof(EcdsaPooledPolicy);
    Console.WriteLine($"[PASS] EcdsaPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}