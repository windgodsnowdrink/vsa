#load "realtime_analytics.cs"

Console.WriteLine("=== realtime_analytics Test ===");

try
{
    var t0 = typeof(RealtimeAnalyticsEngine);
    Console.WriteLine($"[PASS] RealtimeAnalyticsEngine 存在");
    var t1 = typeof(AnalyticsData);
    Console.WriteLine($"[PASS] AnalyticsData record 存在");
    var t2 = typeof(AnalyticsResult);
    Console.WriteLine($"[PASS] AnalyticsResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}