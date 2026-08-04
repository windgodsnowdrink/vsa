#load "tiered_memory_integration.cs"

Console.WriteLine("=== tiered_memory_integration Test ===");

try
{
    var t0 = typeof(DataProcessingService);
    Console.WriteLine($"[PASS] DataProcessingService 存在");
    var t1 = typeof(MemoryMonitorService);
    Console.WriteLine($"[PASS] MemoryMonitorService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}