#load "datadog_integration.cs"

Console.WriteLine("=== datadog_integration Test ===");

try
{
    var t0 = typeof(DataDogMetricsProcessor);
    Console.WriteLine($"[PASS] DataDogMetricsProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}