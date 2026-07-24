#load "dataflow_demo.cs"

Console.WriteLine("=== dataflow_demo Test ===");

try
{
    var t0 = typeof(TodoEvent);
    Console.WriteLine($"[PASS] TodoEvent 存在");
    var t1 = typeof(TodoStreamProcessor);
    Console.WriteLine($"[PASS] TodoStreamProcessor 存在");
    var t2 = typeof(TracingMiddleware);
    Console.WriteLine($"[PASS] TracingMiddleware 存在");
    var t3 = typeof(BatchProcessor);
    Console.WriteLine($"[PASS] BatchProcessor 存在");
    var t4 = typeof(BackpressureStrategy);
    Console.WriteLine($"[PASS] BackpressureStrategy 存在");
    var t5 = typeof(ErrorHandlingStrategy);
    Console.WriteLine($"[PASS] ErrorHandlingStrategy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}