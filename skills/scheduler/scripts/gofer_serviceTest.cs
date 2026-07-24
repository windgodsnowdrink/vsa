#load "gofer_service.cs"

Console.WriteLine("=== gofer_service Test ===");

try
{
    var t0 = typeof(GoferTaskProcessor);
    Console.WriteLine($"[PASS] GoferTaskProcessor 存在");
    var t1 = typeof(TaskContext);
    Console.WriteLine($"[PASS] TaskContext 存在");
    var t2 = typeof(TaskContextPooledPolicy);
    Console.WriteLine($"[PASS] TaskContextPooledPolicy 存在");
    var t3 = typeof(TaskItem);
    Console.WriteLine($"[PASS] TaskItem record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}