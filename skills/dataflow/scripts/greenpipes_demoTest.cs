#load "greenpipes_demo.cs"

Console.WriteLine("=== greenpipes_demo Test ===");

try
{
    var t0 = typeof(TodoEvent);
    Console.WriteLine($"[PASS] TodoEvent 存在");
    var t1 = typeof(ValidateTodoFilter);
    Console.WriteLine($"[PASS] ValidateTodoFilter 存在");
    var t2 = typeof(LogTodoFilter);
    Console.WriteLine($"[PASS] LogTodoFilter 存在");
    var t3 = typeof(DistributedTracingFilter);
    Console.WriteLine($"[PASS] DistributedTracingFilter 存在");
    var t4 = typeof(TodoPipelineBuilder);
    Console.WriteLine($"[PASS] TodoPipelineBuilder 存在");
    var t5 = typeof(DynamicPipelineBuilder);
    Console.WriteLine($"[PASS] DynamicPipelineBuilder 存在");
    var t6 = typeof(CustomMiddleware);
    Console.WriteLine($"[PASS] CustomMiddleware 存在");
    var t7 = typeof(PerformanceMetricsFilter);
    Console.WriteLine($"[PASS] PerformanceMetricsFilter 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}