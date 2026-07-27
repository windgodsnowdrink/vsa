#load "tpl_core.cs"

Console.WriteLine("=== tpl_core Test ===");

try
{
    var t0 = typeof(TPLSkill.TaskService);
    Console.WriteLine($"[PASS] TaskService 存在");
    var t1 = typeof(TPLSkill.DataflowService);
    Console.WriteLine($"[PASS] DataflowService 存在");
    var t2 = typeof(TPLSkill.ParallelService);
    Console.WriteLine($"[PASS] ParallelService 存在");
    var t3 = typeof(TPLSkill.AsyncService);
    Console.WriteLine($"[PASS] AsyncService 存在");
    var t4 = typeof(TPLSkill.ITaskService);
    Console.WriteLine($"[PASS] ITaskService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(TPLSkill.IDataflowService);
    Console.WriteLine($"[PASS] IDataflowService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(TPLSkill.IParallelService);
    Console.WriteLine($"[PASS] IParallelService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(TPLSkill.IAsyncService);
    Console.WriteLine($"[PASS] IAsyncService 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}