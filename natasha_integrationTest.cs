#load "natasha_integration.cs"

Console.WriteLine("=== natasha_integration Test ===");

try
{
    var t0 = typeof(NatashaDynamicService);
    Console.WriteLine($"[PASS] NatashaDynamicService 存在");
    var t1 = typeof(DynamicClass);
    Console.WriteLine($"[PASS] DynamicClass 存在");
    var t2 = typeof(TieredMemoryPool);
    Console.WriteLine($"[PASS] TieredMemoryPool 存在");
    var t3 = typeof(SmallMemoryOwner);
    Console.WriteLine($"[PASS] SmallMemoryOwner 存在");
    var t4 = typeof(StringBuilderPooledPolicy);
    Console.WriteLine($"[PASS] StringBuilderPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}