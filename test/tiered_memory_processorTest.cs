#load "tiered_memory_processor.cs"

Console.WriteLine("=== tiered_memory_processor Test ===");

try
{
    var t0 = typeof(TieredMemoryProcessor);
    Console.WriteLine($"[PASS] TieredMemoryProcessor 存在");
    var t1 = typeof(NativeMemoryPool);
    Console.WriteLine($"[PASS] NativeMemoryPool 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}