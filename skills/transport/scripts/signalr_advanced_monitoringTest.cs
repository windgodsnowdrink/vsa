#load "signalr_advanced_monitoring.cs"

Console.WriteLine("=== signalr_advanced_monitoring Test ===");

try
{
    var t0 = typeof(MonitoringDbContext);
    Console.WriteLine($"[PASS] MonitoringDbContext 存在");
    var t1 = typeof(GitBasedVersioning);
    Console.WriteLine($"[PASS] GitBasedVersioning 存在");
    var t2 = typeof(MonitoringRecord);
    Console.WriteLine($"[PASS] MonitoringRecord record 存在");
    var t3 = typeof(EventTrace);
    Console.WriteLine($"[PASS] EventTrace record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}