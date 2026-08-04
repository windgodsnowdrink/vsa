#load "message_queue.cs"

Console.WriteLine("=== message_queue Test ===");

try
{
    var t0 = typeof(ChannelMessageQueueProcessor);
    Console.WriteLine($"[PASS] ChannelMessageQueueProcessor 存在");
    var t1 = typeof(QueueMessage);
    Console.WriteLine($"[PASS] QueueMessage 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}