#load "fluent_scheduler_service.cs"

Console.WriteLine("=== fluent_scheduler_service Test ===");

try
{
    var t0 = typeof(ScheduledJobProcessor);
    Console.WriteLine($"[PASS] ScheduledJobProcessor 存在");
    var t1 = typeof(FluentSchedulerExtensions);
    Console.WriteLine($"[PASS] FluentSchedulerExtensions 存在");
    var t2 = typeof(FluentSchedulerHostedService);
    Console.WriteLine($"[PASS] FluentSchedulerHostedService 存在");
    var t3 = typeof(JobContext);
    Console.WriteLine($"[PASS] JobContext 存在");
    var t4 = typeof(JobContextPooledPolicy);
    Console.WriteLine($"[PASS] JobContextPooledPolicy 存在");
    var t5 = typeof(SampleJob);
    Console.WriteLine($"[PASS] SampleJob 存在");
    var t6 = typeof(JobItem);
    Console.WriteLine($"[PASS] JobItem record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}