#load "prometheus_metrics.cs"

Console.WriteLine("=== prometheus_metrics Test ===");

try
{
    var t0 = typeof(MetricsRegistry);
    Console.WriteLine($"[PASS] MetricsRegistry 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}