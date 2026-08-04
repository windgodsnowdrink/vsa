#load "acme_certes_integration.cs"

Console.WriteLine("=== acme_certes_integration Test ===");

try
{
    var t0 = typeof(CertesCertificateService);
    Console.WriteLine($"[PASS] CertesCertificateService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}