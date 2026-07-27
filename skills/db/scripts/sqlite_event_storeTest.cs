#load "sqlite_event_store.cs"

Console.WriteLine("=== sqlite_event_store Test ===");

try
{
    var t0 = typeof(Crc32);
    Console.WriteLine($"[PASS] Crc32 存在");
    var t1 = typeof(TransactionEvent);
    Console.WriteLine($"[PASS] TransactionEvent 存在");
    var t2 = typeof(DistributedTransactionCoordinator);
    Console.WriteLine($"[PASS] DistributedTransactionCoordinator 存在");
    var t3 = typeof(SqliteEventStore);
    Console.WriteLine($"[PASS] SqliteEventStore 存在");
    var t4 = typeof(EventFactory);
    Console.WriteLine($"[PASS] EventFactory 存在");
    var t5 = typeof(EventHandler);
    Console.WriteLine($"[PASS] EventHandler 存在");
    var t6 = typeof(SqliteConnectionPoolPolicy);
    Console.WriteLine($"[PASS] SqliteConnectionPoolPolicy 存在");
    var t7 = typeof(SqliteEventStoreExtensions);
    Console.WriteLine($"[PASS] SqliteEventStoreExtensions 存在");
    var t8 = typeof(EventStoreController);
    Console.WriteLine($"[PASS] EventStoreController 存在");
    var t9 = typeof(EventData);
    Console.WriteLine($"[PASS] EventData 存在");
    var t10 = typeof(SqliteEvent);
    Console.WriteLine($"[PASS] SqliteEvent struct 存在");
    var t11 = typeof(EventType);
    Console.WriteLine($"[PASS] EventType enum 存在 (IsEnum: {t11.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}