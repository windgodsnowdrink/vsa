#load "magiconion_chat.cs"

Console.WriteLine("=== magiconion_chat Test ===");

try
{
    var t0 = typeof(ChatService);
    Console.WriteLine($"[PASS] ChatService 存在");
    var t1 = typeof(ChannelChatProcessor);
    Console.WriteLine($"[PASS] ChannelChatProcessor 存在");
    var t2 = typeof(ChatMessage);
    Console.WriteLine($"[PASS] ChatMessage 存在");
    var t3 = typeof(JoinResult);
    Console.WriteLine($"[PASS] JoinResult 存在");
    var t4 = typeof(IChatService);
    Console.WriteLine($"[PASS] IChatService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}