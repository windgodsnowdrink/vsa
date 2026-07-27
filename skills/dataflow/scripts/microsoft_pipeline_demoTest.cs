#load "microsoft_pipeline_demo.cs"

Console.WriteLine("=== microsoft_pipeline_demo Test ===");

try
{
    var t0 = typeof(TodoEvent);
    Console.WriteLine($"[PASS] TodoEvent 存在");
    var t1 = typeof(TodoPipeline);
    Console.WriteLine($"[PASS] TodoPipeline 存在");
    var t2 = typeof(BatchProcessingMiddleware);
    Console.WriteLine($"[PASS] BatchProcessingMiddleware 存在");
    var t3 = typeof(ListPoolPolicy);
    Console.WriteLine($"[PASS] ListPoolPolicy 存在");
    var t4 = typeof(BackpressureChannel);
    Console.WriteLine($"[PASS] BackpressureChannel 存在");
    var t5 = typeof(TracingMiddleware);
    Console.WriteLine($"[PASS] TracingMiddleware 存在");
    var t6 = typeof(ResilientPipeline);
    Console.WriteLine($"[PASS] ResilientPipeline 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}