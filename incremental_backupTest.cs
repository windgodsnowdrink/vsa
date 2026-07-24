#load "incremental_backup.cs"

Console.WriteLine("=== incremental_backup Test ===");

try
{
    var t0 = typeof(IncrementalBackupService);
    Console.WriteLine($"[PASS] IncrementalBackupService 存在");
    var t1 = typeof(IncrementalBackupEngine);
    Console.WriteLine($"[PASS] IncrementalBackupEngine 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}