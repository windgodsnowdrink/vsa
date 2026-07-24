#load "high_frequency_10m.cs"

Console.WriteLine("=== high_frequency_10m Test ===");

try
{
    var t0 = typeof(MessageEventHandler);
    Console.WriteLine($"[PASS] MessageEventHandler 存在");
    var t1 = typeof(LlvmJitCompiler);
    Console.WriteLine($"[PASS] LlvmJitCompiler 存在");
    var t2 = typeof(TradingService);
    Console.WriteLine($"[PASS] TradingService 存在");
    var t3 = typeof(ClusterMessageProcessor);
    Console.WriteLine($"[PASS] ClusterMessageProcessor 存在");
    var t4 = typeof(MessageEvent);
    Console.WriteLine($"[PASS] MessageEvent struct 存在");
    var t5 = typeof(ClusterMessage);
    Console.WriteLine($"[PASS] ClusterMessage struct 存在");
    var t6 = typeof(TradingOrder);
    Console.WriteLine($"[PASS] TradingOrder struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}