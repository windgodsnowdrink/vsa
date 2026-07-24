#load "watchdog_visualization.cs"

Console.WriteLine("=== watchdog_visualization Test ===");

try
{
    var t0 = typeof(MemoryTracker);
    Console.WriteLine($"[PASS] MemoryTracker 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}