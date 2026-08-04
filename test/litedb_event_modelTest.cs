#load "litedb_event_model.cs"

Console.WriteLine("=== litedb_event_model Test ===");

try
{
    var t0 = typeof(EventStoreService);
    Console.WriteLine($"[PASS] EventStoreService 存在");
    var t1 = typeof(ChunkReassembler);
    Console.WriteLine($"[PASS] ChunkReassembler 存在");
    var t2 = typeof(ExtendedPayload);
    Console.WriteLine($"[PASS] ExtendedPayload 存在");
    var t3 = typeof(EventStoreDemo);
    Console.WriteLine($"[PASS] EventStoreDemo 存在");
    var t4 = typeof(TieredEventStorage);
    Console.WriteLine($"[PASS] TieredEventStorage 存在");
    var t5 = typeof(EventData);
    Console.WriteLine($"[PASS] EventData struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}