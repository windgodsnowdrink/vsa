#load "conditional_routing.cs"

Console.WriteLine("=== conditional_routing Test ===");

try
{
    var t0 = typeof(ConditionalRouter);
    Console.WriteLine($"[PASS] ConditionalRouter 存在");
    var t1 = typeof(TaskItem);
    Console.WriteLine($"[PASS] TaskItem record 存在");
    var t2 = typeof(TaskResult);
    Console.WriteLine($"[PASS] TaskResult record 存在");
    var t3 = typeof(Priority);
    Console.WriteLine($"[PASS] Priority enum 存在 (IsEnum: {t3.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}