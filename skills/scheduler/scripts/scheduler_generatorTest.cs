#load "scheduler_generator.cs"

Console.WriteLine("=== scheduler_generator Test ===");

try
{
    var t0 = typeof(Scheduler.Generator.TemplateInfo);
    Console.WriteLine($"[PASS] TemplateInfo 存在");
    var t1 = typeof(Scheduler.Generator.CodeGenerator);
    Console.WriteLine($"[PASS] CodeGenerator 存在");
    var t2 = typeof(Scheduler.Generator.TemplateManager);
    Console.WriteLine($"[PASS] TemplateManager 存在");
    var t3 = typeof(Scheduler.Generator.SchedulerWorker);
    Console.WriteLine($"[PASS] SchedulerWorker 存在");
    var t4 = typeof(Scheduler.Generator.HostCodeGenerator);
    Console.WriteLine($"[PASS] HostCodeGenerator 存在");
    var t5 = typeof(Scheduler.Generator.WorkerCodeGenerator);
    Console.WriteLine($"[PASS] WorkerCodeGenerator 存在");
    var t6 = typeof(Scheduler.Generator.ServiceCodeGenerator);
    Console.WriteLine($"[PASS] ServiceCodeGenerator 存在");
    var t7 = typeof(Scheduler.Generator.CancellationTokenExtensions);
    Console.WriteLine($"[PASS] CancellationTokenExtensions 存在");
    var t8 = typeof(Scheduler.Generator.ICodeGenerator);
    Console.WriteLine($"[PASS] ICodeGenerator 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(Scheduler.Generator.ITemplateManager);
    Console.WriteLine($"[PASS] ITemplateManager 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(Scheduler.Generator.ICodeGeneratorStrategy);
    Console.WriteLine($"[PASS] ICodeGeneratorStrategy 接口存在 (IsInterface: {t10.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}