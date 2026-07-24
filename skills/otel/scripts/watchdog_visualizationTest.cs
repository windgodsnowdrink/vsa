#load "watchdog_visualization.cs"

Console.WriteLine("=== watchdog_visualization Test ===");

try
{
    // 验证 MemoryTracker 类
    var trackerType = typeof(MemoryTracker);
    Console.WriteLine($"[PASS] MemoryTracker 类型存在: {trackerType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}