#load "memory_sse_streaming.cs"

Console.WriteLine("=== memory_sse_streaming Test ===");

try
{
    var t0 = typeof(SseStreamingService);
    Console.WriteLine($"[PASS] SseStreamingService 存在");
    var t1 = typeof(RecyclableMemoryStreamPooledPolicy);
    Console.WriteLine($"[PASS] RecyclableMemoryStreamPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}