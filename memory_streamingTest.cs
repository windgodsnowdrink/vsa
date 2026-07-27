#load "memory_streaming.cs"

Console.WriteLine("=== memory_streaming Test ===");

try
{
    var t0 = typeof(MemoryStreamingService);
    Console.WriteLine($"[PASS] MemoryStreamingService 存在");
    var t1 = typeof(RecyclableMemoryStreamPooledPolicy);
    Console.WriteLine($"[PASS] RecyclableMemoryStreamPooledPolicy 存在");
    var t2 = typeof(StreamData);
    Console.WriteLine($"[PASS] StreamData record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}