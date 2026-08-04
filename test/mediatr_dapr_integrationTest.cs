#load "mediatr_dapr_integration.cs"

Console.WriteLine("=== mediatr_dapr_integration Test ===");

try
{
    var t0 = typeof(DaprEventBusOptions);
    Console.WriteLine($"[PASS] DaprEventBusOptions 存在");
    var t1 = typeof(DaprEventBus);
    Console.WriteLine($"[PASS] DaprEventBus 存在");
    var t2 = typeof(BatchEventProcessor);
    Console.WriteLine($"[PASS] BatchEventProcessor 存在");
    var t3 = typeof(DeadLetterProcessor);
    Console.WriteLine($"[PASS] DeadLetterProcessor 存在");
    var t4 = typeof(DaprEventBusExtensions);
    Console.WriteLine($"[PASS] DaprEventBusExtensions 存在");
    var t5 = typeof(DaprHealthCheck);
    Console.WriteLine($"[PASS] DaprHealthCheck 存在");
    var t6 = typeof(OrderEventHandler);
    Console.WriteLine($"[PASS] OrderEventHandler 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}