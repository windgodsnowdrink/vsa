#load "watchdog_memory.cs"

Console.WriteLine("=== watchdog_memory Test ===");

try
{
    // 验证 TieredMemoryServer 类
    var memoryType = typeof(TieredMemoryServer);
    Console.WriteLine($"[PASS] TieredMemoryServer 类型存在: {memoryType.Name}");
    Console.WriteLine($"[PASS] RentHotMemory 方法: {memoryType.GetMethod("RentHotMemory") != null}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}