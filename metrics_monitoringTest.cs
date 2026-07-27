#load "metrics_monitoring.cs"

Console.WriteLine("=== metrics_monitoring Test ===");

try
{
    var t0 = typeof(CarterMetrics);
    Console.WriteLine($"[PASS] CarterMetrics 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}