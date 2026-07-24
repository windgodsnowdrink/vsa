#load "signalr_integration.cs"

Console.WriteLine("=== signalr_integration Test ===");

try
{
    var t0 = typeof(ChatHub);
    Console.WriteLine($"[PASS] ChatHub 存在");
    var t1 = typeof(ChannelMessageProcessor);
    Console.WriteLine($"[PASS] ChannelMessageProcessor 存在");
    var t2 = typeof(WebSocketMessage);
    Console.WriteLine($"[PASS] WebSocketMessage 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}