#load "disruptor_integration.cs"

Console.WriteLine("=== disruptor_integration Test ===");

try
{
    var t0 = typeof(MessageValidator);
    Console.WriteLine($"[PASS] MessageValidator 存在");
    var t1 = typeof(MessageTransformer);
    Console.WriteLine($"[PASS] MessageTransformer 存在");
    var t2 = typeof(MessagePersister);
    Console.WriteLine($"[PASS] MessagePersister 存在");
    var t3 = typeof(MessageEvent);
    Console.WriteLine($"[PASS] MessageEvent struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}