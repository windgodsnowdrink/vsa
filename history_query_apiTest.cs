#load "history_query_api.cs"

Console.WriteLine("=== history_query_api Test ===");

try
{
    var t0 = typeof(HistoryDbContext);
    Console.WriteLine($"[PASS] HistoryDbContext 存在");
    var t1 = typeof(HistoryCompressionService);
    Console.WriteLine($"[PASS] HistoryCompressionService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}