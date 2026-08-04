#load "watchdog_semantic.cs"

Console.WriteLine("=== watchdog_semantic Test ===");

try
{
    var t0 = typeof(SemanticTracker);
    Console.WriteLine($"[PASS] SemanticTracker 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}