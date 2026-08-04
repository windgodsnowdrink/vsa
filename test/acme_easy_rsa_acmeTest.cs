#load "acme_easy_rsa_acme.cs"

Console.WriteLine("=== acme_easy_rsa_acme Test ===");

try
{
    var t0 = typeof(AcmeCertificateService);
    Console.WriteLine($"[PASS] AcmeCertificateService 存在");
    var t1 = typeof(RsaPooledPolicy);
    Console.WriteLine($"[PASS] RsaPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}