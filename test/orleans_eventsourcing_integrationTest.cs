#load "orleans_eventsourcing_integration.cs"

Console.WriteLine("=== orleans_eventsourcing_integration Test ===");

try
{
    var t0 = typeof(OrderState);
    Console.WriteLine($"[PASS] OrderState 存在");
    var t1 = typeof(OrderGrain);
    Console.WriteLine($"[PASS] OrderGrain 存在");
    var t2 = typeof(OrderEventHandlerGrain);
    Console.WriteLine($"[PASS] OrderEventHandlerGrain 存在");
    var t3 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(OrleansEventSourcingOptions);
    Console.WriteLine($"[PASS] OrleansEventSourcingOptions 存在");
    var t5 = typeof(IOrderGrain);
    Console.WriteLine($"[PASS] IOrderGrain 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(IOrderEventHandlerGrain);
    Console.WriteLine($"[PASS] IOrderEventHandlerGrain 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(EventBase);
    Console.WriteLine($"[PASS] EventBase record 存在");
    var t8 = typeof(OrderCreatedEvent);
    Console.WriteLine($"[PASS] OrderCreatedEvent record 存在");
    var t9 = typeof(OrderPaidEvent);
    Console.WriteLine($"[PASS] OrderPaidEvent record 存在");
    var t10 = typeof(OrderShippedEvent);
    Console.WriteLine($"[PASS] OrderShippedEvent record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}