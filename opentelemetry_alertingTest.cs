#load "opentelemetry_alerting.cs"

Console.WriteLine("=== opentelemetry_alerting Test ===");

try
{
    var t0 = typeof(DynamicAlertEngine);
    Console.WriteLine($"[PASS] DynamicAlertEngine 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}