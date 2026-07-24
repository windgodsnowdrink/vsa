#load "database_backup_service.cs"

Console.WriteLine("=== database_backup_service Test ===");

try
{
    var t0 = typeof(DatabaseBackupService);
    Console.WriteLine($"[PASS] DatabaseBackupService 存在");
    var t1 = typeof(BackupRequest);
    Console.WriteLine($"[PASS] BackupRequest 存在");
    var t2 = typeof(DistributedLockExtensions);
    Console.WriteLine($"[PASS] DistributedLockExtensions 存在");
    var t3 = typeof(BackupStrategy);
    Console.WriteLine($"[PASS] BackupStrategy enum 存在 (IsEnum: {t3.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}