#load "database_backup_service.cs"

Console.WriteLine("=== database_backup_service Test ===");

try
{
    var t0 = typeof(DatabaseBackupService);
    Console.WriteLine($"[PASS] DatabaseBackupService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}