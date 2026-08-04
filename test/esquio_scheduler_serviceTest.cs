#load "esquio_scheduler_service.cs"

Console.WriteLine("=== esquio_scheduler_service Test ===");

try
{
    var t0 = typeof(EsquioScheduler);
    Console.WriteLine($"[PASS] EsquioScheduler 存在");
    var t1 = typeof(JobContext);
    Console.WriteLine($"[PASS] JobContext 存在");
    var t2 = typeof(JobContextPooledPolicy);
    Console.WriteLine($"[PASS] JobContextPooledPolicy 存在");
    var t3 = typeof(ScheduledJob);
    Console.WriteLine($"[PASS] ScheduledJob record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}