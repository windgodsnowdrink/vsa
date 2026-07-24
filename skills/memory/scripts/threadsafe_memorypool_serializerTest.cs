#load "threadsafe_memorypool_serializer.cs"

Console.WriteLine("=== threadsafe_memorypool_serializer Test ===");

try
{
    var t0 = typeof(ThreadSafeMemoryPoolSerializer);
    Console.WriteLine($"[PASS] ThreadSafeMemoryPoolSerializer 存在");
    var t1 = typeof(MessagePackWriterPooledObjectPolicy);
    Console.WriteLine($"[PASS] MessagePackWriterPooledObjectPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}