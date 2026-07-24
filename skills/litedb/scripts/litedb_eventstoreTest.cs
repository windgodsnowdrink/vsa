#load "litedb_eventstore.cs"

Console.WriteLine("=== litedb_eventstore Test ===");

try
{
    var t0 = typeof(OutOfBandEvent);
    Console.WriteLine($"[PASS] OutOfBandEvent 存在");
    var t1 = typeof(EventProcessor);
    Console.WriteLine($"[PASS] EventProcessor 存在");
    var t2 = typeof(BufferPoolPolicy);
    Console.WriteLine($"[PASS] BufferPoolPolicy 存在");
    var t3 = typeof(EventStoreService);
    Console.WriteLine($"[PASS] EventStoreService 存在");
    var t4 = typeof(EventStoreDemo);
    Console.WriteLine($"[PASS] EventStoreDemo 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}