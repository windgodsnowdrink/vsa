#load "message_queue_enhanced.cs"

Console.WriteLine("=== message_queue_enhanced Test ===");

try
{
    var t0 = typeof(ChannelAckProcessor);
    Console.WriteLine($"[PASS] ChannelAckProcessor 存在");
    var t1 = typeof(ChannelDeadLetterProcessor);
    Console.WriteLine($"[PASS] ChannelDeadLetterProcessor 存在");
    var t2 = typeof(DataflowNetwork);
    Console.WriteLine($"[PASS] DataflowNetwork 存在");
    var t3 = typeof(DeadLetterMessage);
    Console.WriteLine($"[PASS] DeadLetterMessage 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}