#load "magiconion_integration.cs"

Console.WriteLine("=== magiconion_integration Test ===");

try
{
    var t0 = typeof(ChannelStreamingProcessor);
    Console.WriteLine($"[PASS] ChannelStreamingProcessor 存在");
    var t1 = typeof(StreamingMessage);
    Console.WriteLine($"[PASS] StreamingMessage 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}