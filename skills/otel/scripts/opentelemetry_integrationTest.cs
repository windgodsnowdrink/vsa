#load "opentelemetry_integration.cs"

Console.WriteLine("=== opentelemetry_integration Test ===");

try
{
    var t0 = typeof(OtelMetricsProcessor);
    Console.WriteLine($"[PASS] OtelMetricsProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}