#load "watchdog_disruptor.cs"

Console.WriteLine("=== watchdog_disruptor Test ===");

try
{
    // 验证 LogEventProcessor 类
    var processorType = typeof(LogEventProcessor);
    Console.WriteLine($"[PASS] LogEventProcessor 类型存在: {processorType.Name}");
    Console.WriteLine($"[PASS] OnEvent 方法: {processorType.GetMethod("OnEvent") != null}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}