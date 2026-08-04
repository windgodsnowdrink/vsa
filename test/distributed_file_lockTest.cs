#load "distributed_file_lock.cs"

Console.WriteLine("=== distributed_file_lock Test ===");

try
{
    var t0 = typeof(FileLockService);
    Console.WriteLine($"[PASS] FileLockService 存在");
    var t1 = typeof(DistributedLockService);
    Console.WriteLine($"[PASS] DistributedLockService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}