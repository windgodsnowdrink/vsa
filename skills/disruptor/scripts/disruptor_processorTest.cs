#load "disruptor_processor.cs"

Console.WriteLine("=== disruptor_processor Test ===");

try
{
    var t0 = typeof(DisruptorProcessor);
    Console.WriteLine($"[PASS] DisruptorProcessor 存在");
    var t1 = typeof(LogEventProcessor);
    Console.WriteLine($"[PASS] LogEventProcessor 存在");
    var t2 = typeof(TaskItem);
    Console.WriteLine($"[PASS] TaskItem record 存在");
    var t3 = typeof(TaskResult);
    Console.WriteLine($"[PASS] TaskResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}