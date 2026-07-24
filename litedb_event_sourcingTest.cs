#load "litedb_event_sourcing.cs"

Console.WriteLine("=== litedb_event_sourcing Test ===");

try
{
    var t0 = typeof(AggregateRoot);
    Console.WriteLine($"[PASS] AggregateRoot 存在");
    var t1 = typeof(EventStoreEngine);
    Console.WriteLine($"[PASS] EventStoreEngine 存在");
    var t2 = typeof(EventPersistHandler);
    Console.WriteLine($"[PASS] EventPersistHandler 存在");
    var t3 = typeof(Order);
    Console.WriteLine($"[PASS] Order 存在");
    var t4 = typeof(OrderCreatedEvent);
    Console.WriteLine($"[PASS] OrderCreatedEvent 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}