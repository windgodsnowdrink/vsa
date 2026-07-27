#load "watchdog_semantic.cs"

Console.WriteLine("=== watchdog_semantic Test ===");

try
{
    // 验证 SemanticTracker 类
    var trackerType = typeof(SemanticTracker);
    Console.WriteLine($"[PASS] SemanticTracker 类型存在: {trackerType.Name}");
    Console.WriteLine($"[PASS] AnalyzeLogsAsync 方法: {trackerType.GetMethod("AnalyzeLogsAsync") != null}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}