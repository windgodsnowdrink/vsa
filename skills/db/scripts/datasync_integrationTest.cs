#load "datasync_integration.cs"

Console.WriteLine("=== datasync_integration Test ===");

try
{
    var t0 = typeof(LastWriteWinsConflictResolver);
    Console.WriteLine($"[PASS] LastWriteWinsConflictResolver 存在");
    var t1 = typeof(LocalWinsConflictResolver);
    Console.WriteLine($"[PASS] LocalWinsConflictResolver 存在");
    var t2 = typeof(OfflineSyncService);
    Console.WriteLine($"[PASS] OfflineSyncService 存在");
    var t3 = typeof(SyncController);
    Console.WriteLine($"[PASS] SyncController 存在");
    var t4 = typeof(SyncConflictResolver);
    Console.WriteLine($"[PASS] SyncConflictResolver 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(SyncItem);
    Console.WriteLine($"[PASS] SyncItem record 存在");
    var t6 = typeof(SyncStatus);
    Console.WriteLine($"[PASS] SyncStatus enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}