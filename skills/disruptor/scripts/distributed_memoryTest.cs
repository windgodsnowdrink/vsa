#load "distributed_memory.cs"

Console.WriteLine("=== distributed_memory Test ===");

try
{
    var t0 = typeof(ChannelDistributedMemoryHandler);
    Console.WriteLine($"[PASS] ChannelDistributedMemoryHandler 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}