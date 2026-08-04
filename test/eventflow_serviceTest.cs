#load "eventflow_service.cs"

Console.WriteLine("=== eventflow_service Test ===");

try
{
    var t0 = typeof(OrderAggregate);
    Console.WriteLine($"[PASS] OrderAggregate 存在");
    var t1 = typeof(OrderCreatedEvent);
    Console.WriteLine($"[PASS] OrderCreatedEvent 存在");
    var t2 = typeof(EventFlowProcessor);
    Console.WriteLine($"[PASS] EventFlowProcessor 存在");
    var t3 = typeof(EventContext);
    Console.WriteLine($"[PASS] EventContext 存在");
    var t4 = typeof(EventContextPooledPolicy);
    Console.WriteLine($"[PASS] EventContextPooledPolicy 存在");
    var t5 = typeof(OrderCommand);
    Console.WriteLine($"[PASS] OrderCommand record 存在");
    var t6 = typeof(OrderId);
    Console.WriteLine($"[PASS] OrderId record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}