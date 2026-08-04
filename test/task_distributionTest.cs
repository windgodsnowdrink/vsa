#load "task_distribution.cs"

Console.WriteLine("=== task_distribution Test ===");

try
{
    var t0 = typeof(TaskDistributor);
    Console.WriteLine($"[PASS] TaskDistributor 存在");
    var t1 = typeof(TaskBatch);
    Console.WriteLine($"[PASS] TaskBatch record 存在");
    var t2 = typeof(TaskItem);
    Console.WriteLine($"[PASS] TaskItem record 存在");
    var t3 = typeof(TaskResult);
    Console.WriteLine($"[PASS] TaskResult record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}