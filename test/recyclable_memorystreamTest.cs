#load "recyclable_memorystream.cs"

Console.WriteLine("=== recyclable_memorystream Test ===");

try
{
    var t0 = typeof(ChannelMemoryProcessor);
    Console.WriteLine($"[PASS] ChannelMemoryProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}