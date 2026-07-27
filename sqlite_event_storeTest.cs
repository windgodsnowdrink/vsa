#load "sqlite_event_store.cs"

Console.WriteLine("=== sqlite_event_store Test ===");

try
{
    var t0 = typeof(DistributedTransactionCoordinator);
    Console.WriteLine($"[PASS] DistributedTransactionCoordinator 存在");
    var t1 = typeof(SqliteEventStore);
    Console.WriteLine($"[PASS] SqliteEventStore 存在");
    var t2 = typeof(SqliteEventHandler);
    Console.WriteLine($"[PASS] SqliteEventHandler 存在");
    var t3 = typeof(SqliteEventPoolPolicy);
    Console.WriteLine($"[PASS] SqliteEventPoolPolicy 存在");
    var t4 = typeof(SqliteCommandPoolPolicy);
    Console.WriteLine($"[PASS] SqliteCommandPoolPolicy 存在");
    var t5 = typeof(SqliteEvent);
    Console.WriteLine($"[PASS] SqliteEvent struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}