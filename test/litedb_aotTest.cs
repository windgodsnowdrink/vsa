#load "litedb_aot.cs"

Console.WriteLine("=== litedb_aot Test ===");

try
{
    var t0 = typeof(AotOptimizedRepository);
    Console.WriteLine($"[PASS] AotOptimizedRepository 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}