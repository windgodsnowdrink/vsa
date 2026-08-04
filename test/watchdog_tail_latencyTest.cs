#load "watchdog_tail_latency.cs"

Console.WriteLine("=== watchdog_tail_latency Test ===");

try
{
    var t0 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}