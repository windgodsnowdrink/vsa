#load "grpc_protobufnet.cs"

Console.WriteLine("=== grpc_protobufnet Test ===");

try
{
    var t0 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t1 = typeof(TodoService);
    Console.WriteLine($"[PASS] TodoService 存在");
    var t2 = typeof(TodoChannelProcessor);
    Console.WriteLine($"[PASS] TodoChannelProcessor 存在");
    var t3 = typeof(TracingInterceptor);
    Console.WriteLine($"[PASS] TracingInterceptor 存在");
    var t4 = typeof(ChaosInterceptor);
    Console.WriteLine($"[PASS] ChaosInterceptor 存在");
    var t5 = typeof(ITodoService);
    Console.WriteLine($"[PASS] ITodoService 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}