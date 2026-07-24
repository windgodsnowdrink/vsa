#load "mysql_to_sqlite_litedb_sync.cs"

Console.WriteLine("=== mysql_to_sqlite_litedb_sync Test ===");

try
{
    var t0 = typeof(DataSyncService);
    Console.WriteLine($"[PASS] DataSyncService 存在");
    var t1 = typeof(SyncData);
    Console.WriteLine($"[PASS] SyncData 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(LiteDBSyncStateStore);
    Console.WriteLine($"[PASS] LiteDBSyncStateStore 存在");
    var t4 = typeof(SyncState);
    Console.WriteLine($"[PASS] SyncState 存在");
    var t5 = typeof(ISyncStateStore);
    Console.WriteLine($"[PASS] ISyncStateStore 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}