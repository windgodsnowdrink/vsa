#load "metrics_enhanced.cs"

Console.WriteLine("=== metrics_enhanced Test ===");

try
{
    var t0 = typeof(ChannelMetricProcessor);
    Console.WriteLine($"[PASS] ChannelMetricProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}