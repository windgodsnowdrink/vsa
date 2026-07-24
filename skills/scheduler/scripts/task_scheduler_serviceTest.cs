#load "task_scheduler_service.cs"

Console.WriteLine("=== task_scheduler_service Test ===");

try
{
    var t0 = typeof(TaskSchedulerService);
    Console.WriteLine($"[PASS] TaskSchedulerService 存在");
    var t1 = typeof(TaskContext);
    Console.WriteLine($"[PASS] TaskContext 存在");
    var t2 = typeof(TaskContextPooledPolicy);
    Console.WriteLine($"[PASS] TaskContextPooledPolicy 存在");
    var t3 = typeof(ScheduledTask);
    Console.WriteLine($"[PASS] ScheduledTask record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}