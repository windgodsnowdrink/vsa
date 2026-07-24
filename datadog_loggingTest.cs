#load "datadog_logging.cs"

Console.WriteLine("=== datadog_logging Test ===");

try
{
    var t0 = typeof(LogEnricher);
    Console.WriteLine($"[PASS] LogEnricher 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}