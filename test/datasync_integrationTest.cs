#load "datasync_integration.cs"

Console.WriteLine("=== datasync_integration Test ===");

try
{
    var t0 = typeof(DataSyncDemo.OfflineSyncService);
    Console.WriteLine($"[PASS] OfflineSyncService 存在");
    var t1 = typeof(DataSyncDemo.SyncConflictResolver);
    Console.WriteLine($"[PASS] SyncConflictResolver 存在");
    var t2 = typeof(DataSyncDemo.DeltaSyncProvider);
    Console.WriteLine($"[PASS] DeltaSyncProvider 存在");
    var t3 = typeof(DataSyncDemo.SyncPerformanceMonitor);
    Console.WriteLine($"[PASS] SyncPerformanceMonitor 存在");
    var t4 = typeof(DataSyncDemo.DataSyncService);
    Console.WriteLine($"[PASS] DataSyncService 存在");
    var t5 = typeof(DataSyncDemo.DataSyncExtensions);
    Console.WriteLine($"[PASS] DataSyncExtensions 存在");
    var t6 = typeof(DataSyncDemo.DataSyncOptions);
    Console.WriteLine($"[PASS] DataSyncOptions 存在");
    var t7 = typeof(DataSyncDemo.DataSyncBackgroundService);
    Console.WriteLine($"[PASS] DataSyncBackgroundService 存在");
    var t8 = typeof(DataSyncDemo.SyncItem);
    Console.WriteLine($"[PASS] SyncItem 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}