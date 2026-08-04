#load "hangfire_job_service.cs"

Console.WriteLine("=== hangfire_job_service Test ===");

try
{
    var t0 = typeof(JobProcessor);
    Console.WriteLine($"[PASS] JobProcessor 存在");
    var t1 = typeof(HangfireServiceExtensions);
    Console.WriteLine($"[PASS] HangfireServiceExtensions 存在");
    var t2 = typeof(CronScheduler);
    Console.WriteLine($"[PASS] CronScheduler 存在");
    var t3 = typeof(HangfireOptions);
    Console.WriteLine($"[PASS] HangfireOptions 存在");
    var t4 = typeof(JobItem);
    Console.WriteLine($"[PASS] JobItem record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}