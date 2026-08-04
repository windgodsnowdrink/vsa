#load "watchdog_memory.cs"

Console.WriteLine("=== watchdog_memory Test ===");

try
{
    var t0 = typeof(TieredMemoryServer);
    Console.WriteLine($"[PASS] TieredMemoryServer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}