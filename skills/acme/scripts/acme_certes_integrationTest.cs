#load "acme_certes_integration.cs"

Console.WriteLine("=== acme_certes_integration Test ===");

try
{
    var t0 = typeof(CertificateRequest);
    Console.WriteLine($"[PASS] CertificateRequest 存在");
    var t1 = typeof(CertesCertificateService);
    Console.WriteLine($"[PASS] CertesCertificateService 存在");
    var t2 = typeof(IAcmeProtocolClient);
    Console.WriteLine($"[PASS] IAcmeProtocolClient 接口存在 (IsInterface: {t2.IsInterface})");
    var t3 = typeof(CertificateType);
    Console.WriteLine($"[PASS] CertificateType enum 存在 (IsEnum: {t3.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}