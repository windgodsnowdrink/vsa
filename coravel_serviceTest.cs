#load "coravel_service.cs"

Console.WriteLine("=== coravel_service Test ===");

try
{
    var t0 = typeof(CoravelQueueProcessor);
    Console.WriteLine($"[PASS] CoravelQueueProcessor 存在");
    var t1 = typeof(QueueContext);
    Console.WriteLine($"[PASS] QueueContext 存在");
    var t2 = typeof(QueueContextPooledPolicy);
    Console.WriteLine($"[PASS] QueueContextPooledPolicy 存在");
    var t3 = typeof(SampleJob);
    Console.WriteLine($"[PASS] SampleJob 存在");
    var t4 = typeof(QueueItem);
    Console.WriteLine($"[PASS] QueueItem record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}