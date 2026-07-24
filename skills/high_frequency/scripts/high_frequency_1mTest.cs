#load "high_frequency_1m.cs"

Console.WriteLine("=== high_frequency_1m Test ===");

try
{
    var t0 = typeof(AeronMessageProcessor);
    Console.WriteLine($"[PASS] AeronMessageProcessor 存在");
    var t1 = typeof(MessageProcessor);
    Console.WriteLine($"[PASS] MessageProcessor 存在");
    var t2 = typeof(ThreadLocalAllocator);
    Console.WriteLine($"[PASS] ThreadLocalAllocator 存在");
    var t3 = typeof(MessageEnvelope);
    Console.WriteLine($"[PASS] MessageEnvelope struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}