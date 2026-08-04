#load "threadsafe_memorypool_optimized.cs"

Console.WriteLine("=== threadsafe_memorypool_optimized Test ===");

try
{
    var t0 = typeof(ThreadSafeMemoryPoolOptimized);
    Console.WriteLine($"[PASS] ThreadSafeMemoryPoolOptimized 存在");
    var t1 = typeof(BufferPooledObjectPolicy);
    Console.WriteLine($"[PASS] BufferPooledObjectPolicy 存在");
    var t2 = typeof(JsonWriterPooledObjectPolicy);
    Console.WriteLine($"[PASS] JsonWriterPooledObjectPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}