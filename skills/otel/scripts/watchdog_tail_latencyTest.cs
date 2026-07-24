#load "watchdog_tail_latency.cs"

Console.WriteLine("=== watchdog_tail_latency Test ===");

try
{
    // 验证 TailLatencyOptimizer 类
    var optimizerType = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 类型存在: {optimizerType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}