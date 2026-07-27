#load "fastendpoints.cs"

Console.WriteLine("=== fastendpoints Test ===");

try
{
    var t0 = typeof(FastEndpointsExample.TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t1 = typeof(FastEndpointsExample.TodoDbContext);
    Console.WriteLine($"[PASS] TodoDbContext 存在");
    var t2 = typeof(FastEndpointsExample.SpanMemoryOptimizer);
    Console.WriteLine($"[PASS] SpanMemoryOptimizer 存在");
    var t3 = typeof(FastEndpointsExample.TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t4 = typeof(FastEndpointsExample.CreateTodoRequest);
    Console.WriteLine($"[PASS] CreateTodoRequest 存在");
    var t5 = typeof(FastEndpointsExample.UpdateTodoRequest);
    Console.WriteLine($"[PASS] UpdateTodoRequest 存在");
    var t6 = typeof(FastEndpointsExample.TodoResponse);
    Console.WriteLine($"[PASS] TodoResponse 存在");
    var t7 = typeof(FastEndpointsExample.TodoEndpoint);
    Console.WriteLine($"[PASS] TodoEndpoint 存在");
    var t8 = typeof(FastEndpointsExample.IMemoryOptimizer);
    Console.WriteLine($"[PASS] IMemoryOptimizer 接口存在 (IsInterface: {t8.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}