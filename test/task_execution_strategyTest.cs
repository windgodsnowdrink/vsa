#load "task_execution_strategy.cs"

Console.WriteLine("=== task_execution_strategy Test ===");

try
{
    var t0 = typeof(TaskExecutionStrategy);
    Console.WriteLine($"[PASS] TaskExecutionStrategy 存在");
    var t1 = typeof(TaskItem);
    Console.WriteLine($"[PASS] TaskItem record 存在");
    var t2 = typeof(TaskResult);
    Console.WriteLine($"[PASS] TaskResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}