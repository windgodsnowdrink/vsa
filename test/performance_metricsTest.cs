#load "performance_metrics.cs"

Console.WriteLine("=== performance_metrics Test ===");

try
{
    var t0 = typeof(ChannelPerformanceMonitor);
    Console.WriteLine($"[PASS] ChannelPerformanceMonitor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}