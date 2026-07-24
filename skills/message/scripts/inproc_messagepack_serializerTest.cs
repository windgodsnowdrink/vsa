#load "inproc_messagepack_serializer.cs"

Console.WriteLine("=== inproc_messagepack_serializer Test ===");

try
{
    var t0 = typeof(InProcMessagePackSerializer);
    Console.WriteLine($"[PASS] InProcMessagePackSerializer 存在");
    var t1 = typeof(MemoryStreamPooledObjectPolicy);
    Console.WriteLine($"[PASS] MemoryStreamPooledObjectPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}