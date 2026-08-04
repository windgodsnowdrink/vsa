#load "acme_lettuce_encrypt.cs"

Console.WriteLine("=== acme_lettuce_encrypt Test ===");

try
{
    var t0 = typeof(CertificateManager);
    Console.WriteLine($"[PASS] CertificateManager 存在");
    var t1 = typeof(CertificateRenewalService);
    Console.WriteLine($"[PASS] CertificateRenewalService 存在");
    var t2 = typeof(CertificatePooledPolicy);
    Console.WriteLine($"[PASS] CertificatePooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}