#load "opentelemetry_prometheus.cs"

Console.WriteLine("=== opentelemetry_prometheus Test ===");

try
{
    var t0 = typeof(ThreadLocalMetricExporter);
    Console.WriteLine($"[PASS] ThreadLocalMetricExporter 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}