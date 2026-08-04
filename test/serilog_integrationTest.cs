#load "serilog_integration.cs"

Console.WriteLine("=== serilog_integration Test ===");

try
{
    var t0 = typeof(SerilogExtensibilityExtensions);
    Console.WriteLine($"[PASS] SerilogExtensibilityExtensions 存在");
    var t1 = typeof(HighPerformanceLogQueue);
    Console.WriteLine($"[PASS] HighPerformanceLogQueue 存在");
    var t2 = typeof(DataMaskingExtensions);
    Console.WriteLine($"[PASS] DataMaskingExtensions 存在");
    var t3 = typeof(DataMaskingEnricher);
    Console.WriteLine($"[PASS] DataMaskingEnricher 存在");
    var t4 = typeof(SerilogExtensibilityProductionExtensions);
    Console.WriteLine($"[PASS] SerilogExtensibilityProductionExtensions 存在");
    var t5 = typeof(ISerilogPlugin);
    Console.WriteLine($"[PASS] ISerilogPlugin 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}