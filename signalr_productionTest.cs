#load "signalr_production.cs"

Console.WriteLine("=== signalr_production Test ===");

try
{
    var t0 = typeof(ChatHub);
    Console.WriteLine($"[PASS] ChatHub 存在");
    var t1 = typeof(NotificationHub);
    Console.WriteLine($"[PASS] NotificationHub 存在");
    var t2 = typeof(MessageDbContext);
    Console.WriteLine($"[PASS] MessageDbContext 存在");
    var t3 = typeof(ChatMessage);
    Console.WriteLine($"[PASS] ChatMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}