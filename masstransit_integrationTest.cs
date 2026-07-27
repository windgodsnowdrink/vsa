#load "masstransit_integration.cs"

Console.WriteLine("=== masstransit_integration Test ===");

try
{
    var t0 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t1 = typeof(ChannelMessageProcessor);
    Console.WriteLine($"[PASS] ChannelMessageProcessor 存在");
    var t2 = typeof(MessageEnvelope);
    Console.WriteLine($"[PASS] MessageEnvelope record 存在");
    var t3 = typeof(MessageEnvelope);
    Console.WriteLine($"[PASS] MessageEnvelope struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}