#load "longchat_integration.cs"

Console.WriteLine("=== longchat_integration Test ===");

try
{
    var t0 = typeof(LongChatIntegration.LongChatOptions);
    Console.WriteLine($"[PASS] LongChatOptions 存在");
    var t1 = typeof(LongChatIntegration.LongChatService);
    Console.WriteLine($"[PASS] LongChatService 存在");
    var t2 = typeof(LongChatIntegration.LongChatExtensions);
    Console.WriteLine($"[PASS] LongChatExtensions 存在");
    var t3 = typeof(LongChatIntegration.ILongChatService);
    Console.WriteLine($"[PASS] ILongChatService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}