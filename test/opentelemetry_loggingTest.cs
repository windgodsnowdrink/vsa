#load "opentelemetry_logging.cs"

Console.WriteLine("=== opentelemetry_logging Test ===");

try
{
    var t0 = typeof(TraceContextEnricher);
    Console.WriteLine($"[PASS] TraceContextEnricher 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}