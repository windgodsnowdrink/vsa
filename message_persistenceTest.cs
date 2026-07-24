#load "message_persistence.cs"

Console.WriteLine("=== message_persistence Test ===");

try
{
    var t0 = typeof(ChannelMessagePersister);
    Console.WriteLine($"[PASS] ChannelMessagePersister 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}