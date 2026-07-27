#load "recyclable_memorystream_integration.cs"

Console.WriteLine("=== recyclable_memorystream_integration Test ===");

try
{
    var t0 = typeof(RecyclableMemoryStreamOptions);
    Console.WriteLine($"[PASS] RecyclableMemoryStreamOptions 存在");
    var t1 = typeof(MemoryStreamPoolService);
    Console.WriteLine($"[PASS] MemoryStreamPoolService 存在");
    var t2 = typeof(RecyclableMemoryStreamExtensions);
    Console.WriteLine($"[PASS] RecyclableMemoryStreamExtensions 存在");
    var t3 = typeof(RecyclableMemoryStreamExample);
    Console.WriteLine($"[PASS] RecyclableMemoryStreamExample 存在");
    var t4 = typeof(IMemoryStreamPoolService);
    Console.WriteLine($"[PASS] IMemoryStreamPoolService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}