#load "masstransit_inmemory_integration.cs"

Console.WriteLine("=== masstransit_inmemory_integration Test ===");

try
{
    var t0 = typeof(InMemoryMessageConsumer);
    Console.WriteLine($"[PASS] InMemoryMessageConsumer 存在");
    var t1 = typeof(InMemoryMessage);
    Console.WriteLine($"[PASS] InMemoryMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}