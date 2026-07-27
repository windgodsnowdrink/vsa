#load "livechat_integration.cs"

Console.WriteLine("=== livechat_integration Test ===");

try
{
    var t0 = typeof(LiveStreamProcessor);
    Console.WriteLine($"[PASS] LiveStreamProcessor 存在");
    var t1 = typeof(LiveChatHub);
    Console.WriteLine($"[PASS] LiveChatHub 存在");
    var t2 = typeof(LiveStreamBackgroundService);
    Console.WriteLine($"[PASS] LiveStreamBackgroundService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}