#load "shared_memory_serializer.cs"

Console.WriteLine("=== shared_memory_serializer Test ===");

try
{
    var t0 = typeof(SharedMemorySerializer);
    Console.WriteLine($"[PASS] SharedMemorySerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}