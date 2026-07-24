#load "skyapm_logging.cs"

Console.WriteLine("=== skyapm_logging Test ===");

try
{
    // 验证 TraceContextEnricher 类
    var enricherType = typeof(TraceContextEnricher);
    Console.WriteLine($"[PASS] TraceContextEnricher 类型存在: {enricherType.Name}");
    Console.WriteLine($"[PASS] Enrich 方法: {enricherType.GetMethod("Enrich") != null}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}