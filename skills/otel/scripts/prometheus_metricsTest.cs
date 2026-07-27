#load "prometheus_metrics.cs"

Console.WriteLine("=== prometheus_metrics Test ===");

try
{
    // 验证 MetricsRegistry 类
    var registryType = typeof(MetricsRegistry);
    Console.WriteLine($"[PASS] MetricsRegistry 类型存在: {registryType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}