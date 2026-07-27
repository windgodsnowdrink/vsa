#load "pipelinenet_demo.cs"

Console.WriteLine("=== pipelinenet_demo Test ===");

try
{
    var t0 = typeof(TodoEvent);
    Console.WriteLine($"[PASS] TodoEvent 存在");
    var t1 = typeof(ValidationMiddleware);
    Console.WriteLine($"[PASS] ValidationMiddleware 存在");
    var t2 = typeof(LoggingMiddleware);
    Console.WriteLine($"[PASS] LoggingMiddleware 存在");
    var t3 = typeof(TodoPipelineProcessor);
    Console.WriteLine($"[PASS] TodoPipelineProcessor 存在");
    var t4 = typeof(BatchProcessingMiddleware);
    Console.WriteLine($"[PASS] BatchProcessingMiddleware 存在");
    var t5 = typeof(ListPoolPolicy);
    Console.WriteLine($"[PASS] ListPoolPolicy 存在");
    var t6 = typeof(ResilientMiddleware);
    Console.WriteLine($"[PASS] ResilientMiddleware 存在");
    var t7 = typeof(TracingMiddleware);
    Console.WriteLine($"[PASS] TracingMiddleware 存在");
    var t8 = typeof(PipelineBuilderExtensions);
    Console.WriteLine($"[PASS] PipelineBuilderExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}