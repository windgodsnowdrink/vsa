#load "shared_memory_messagepack.cs"

Console.WriteLine("=== shared_memory_messagepack Test ===");

try
{
    var t0 = typeof(SharedMemoryMessagePackSerializer);
    Console.WriteLine($"[PASS] SharedMemoryMessagePackSerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}