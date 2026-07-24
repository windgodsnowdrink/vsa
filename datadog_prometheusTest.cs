#load "datadog_prometheus.cs"

Console.WriteLine("=== datadog_prometheus Test ===");

try
{
    var t0 = typeof(PrometheusExporter);
    Console.WriteLine($"[PASS] PrometheusExporter 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}