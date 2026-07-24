#load "brighter_integration.cs"

Console.WriteLine("=== brighter_integration Test ===");

try
{
    var t0 = typeof(ConsistentHashShardingStrategy);
    Console.WriteLine($"[PASS] ConsistentHashShardingStrategy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}