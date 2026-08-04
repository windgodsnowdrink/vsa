#load "watchdog_disruptor.cs"

Console.WriteLine("=== watchdog_disruptor Test ===");

try
{
    var t0 = typeof(LogEventProcessor);
    Console.WriteLine($"[PASS] LogEventProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}