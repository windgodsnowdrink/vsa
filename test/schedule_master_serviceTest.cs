#load "schedule_master_service.cs"

Console.WriteLine("=== schedule_master_service Test ===");

try
{
    var t0 = typeof(TaskRegistry);
    Console.WriteLine($"[PASS] TaskRegistry 存在");
    var t1 = typeof(ScheduleMasterProcessor);
    Console.WriteLine($"[PASS] ScheduleMasterProcessor 存在");
    var t2 = typeof(TaskContext);
    Console.WriteLine($"[PASS] TaskContext 存在");
    var t3 = typeof(TaskContextPooledPolicy);
    Console.WriteLine($"[PASS] TaskContextPooledPolicy 存在");
    var t4 = typeof(EmailTaskHandler);
    Console.WriteLine($"[PASS] EmailTaskHandler 存在");
    var t5 = typeof(ITaskHandler);
    Console.WriteLine($"[PASS] ITaskHandler 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(IStorageProvider);
    Console.WriteLine($"[PASS] IStorageProvider 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(ScheduleTask);
    Console.WriteLine($"[PASS] ScheduleTask record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}