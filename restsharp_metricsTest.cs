#load "restsharp_metrics.cs"

Console.WriteLine("=== restsharp_metrics Test ===");

try
{
    var t0 = typeof(PrometheusMetricCollector);
    Console.WriteLine($"[PASS] PrometheusMetricCollector 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}