#load "distributed_bus.cs"

Console.WriteLine("=== distributed_bus Test ===");

try
{
    var t0 = typeof(DistributedMessageBus);
    Console.WriteLine($"[PASS] DistributedMessageBus 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}