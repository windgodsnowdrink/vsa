#load "scheduler_core.cs"

Console.WriteLine("=== scheduler_core Test ===");

try
{
    var t0 = typeof(Scheduler.Core.ScheduleTask);
    Console.WriteLine($"[PASS] ScheduleTask 存在");
    var t1 = typeof(Scheduler.Core.TaskExecutionResult);
    Console.WriteLine($"[PASS] TaskExecutionResult 存在");
    var t2 = typeof(Scheduler.Core.SchedulerStatus);
    Console.WriteLine($"[PASS] SchedulerStatus 存在");
    var t3 = typeof(Scheduler.Core.TaskExecutionRecord);
    Console.WriteLine($"[PASS] TaskExecutionRecord 存在");
    var t4 = typeof(Scheduler.Core.SchedulerService);
    Console.WriteLine($"[PASS] SchedulerService 存在");
    var t5 = typeof(Scheduler.Core.TaskExecutor);
    Console.WriteLine($"[PASS] TaskExecutor 存在");
    var t6 = typeof(Scheduler.Core.FileJobStore);
    Console.WriteLine($"[PASS] FileJobStore 存在");
    var t7 = typeof(Scheduler.Core.CronParser);
    Console.WriteLine($"[PASS] CronParser 存在");
    var t8 = typeof(Scheduler.Core.DelayScheduler);
    Console.WriteLine($"[PASS] DelayScheduler 存在");
    var t9 = typeof(Scheduler.Core.IntervalScheduler);
    Console.WriteLine($"[PASS] IntervalScheduler 存在");
    var t10 = typeof(Scheduler.Core.SchedulerHostedService);
    Console.WriteLine($"[PASS] SchedulerHostedService 存在");
    var t11 = typeof(Scheduler.Core.CancellationTokenExtensions);
    Console.WriteLine($"[PASS] CancellationTokenExtensions 存在");
    var t12 = typeof(Scheduler.Core.LoggingExtensions);
    Console.WriteLine($"[PASS] LoggingExtensions 存在");
    var t13 = typeof(Scheduler.Core.ISchedulerService);
    Console.WriteLine($"[PASS] ISchedulerService 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(Scheduler.Core.ITaskExecutor);
    Console.WriteLine($"[PASS] ITaskExecutor 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(Scheduler.Core.IJobStore);
    Console.WriteLine($"[PASS] IJobStore 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(Scheduler.Core.ICronParser);
    Console.WriteLine($"[PASS] ICronParser 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(Scheduler.Core.IDelayScheduler);
    Console.WriteLine($"[PASS] IDelayScheduler 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(Scheduler.Core.IIntervalScheduler);
    Console.WriteLine($"[PASS] IIntervalScheduler 接口存在 (IsInterface: {t18.IsInterface})");
    var t19 = typeof(Scheduler.Core.ISchedulerHostedService);
    Console.WriteLine($"[PASS] ISchedulerHostedService 接口存在 (IsInterface: {t19.IsInterface})");
    var t20 = typeof(Scheduler.Core.TaskStatus);
    Console.WriteLine($"[PASS] TaskStatus enum 存在 (IsEnum: {t20.IsEnum})");
    var t21 = typeof(Scheduler.Core.TaskExecutionStatus);
    Console.WriteLine($"[PASS] TaskExecutionStatus enum 存在 (IsEnum: {t21.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}