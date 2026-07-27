#load "litedb_tieredmemory.cs"

Console.WriteLine("=== litedb_tieredmemory Test ===");

try
{
    var t0 = typeof(TieredMemoryStorage);
    Console.WriteLine($"[PASS] TieredMemoryStorage 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}