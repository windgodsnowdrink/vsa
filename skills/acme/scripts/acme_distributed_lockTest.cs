#load "acme_distributed_lock.cs"

Console.WriteLine("=== acme_distributed_lock Test ===");

try
{
    var t0 = typeof(CertificateLockService);
    Console.WriteLine($"[PASS] CertificateLockService 存在");
    var t1 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t2 = typeof(TailLatencyToken);
    Console.WriteLine($"[PASS] TailLatencyToken 存在");
    var t3 = typeof(RedisLock);
    Console.WriteLine($"[PASS] RedisLock struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}