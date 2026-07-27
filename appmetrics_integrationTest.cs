#load "appmetrics_integration.cs"

Console.WriteLine("=== appmetrics_integration Test ===");

try
{
    var t0 = typeof(AppMetricsService);
    Console.WriteLine($"[PASS] AppMetricsService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}