#load "acme_cert_monitor.cs"

Console.WriteLine("=== acme_cert_monitor Test ===");

try
{
    var t0 = typeof(CertificateMonitor);
    Console.WriteLine($"[PASS] CertificateMonitor 存在");
    var t1 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t2 = typeof(TailLatencyToken);
    Console.WriteLine($"[PASS] TailLatencyToken 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}