#load "masstransit_production_integration.cs"

Console.WriteLine("=== masstransit_production_integration Test ===");

try
{
    var t0 = typeof(MassTransitOptions);
    Console.WriteLine($"[PASS] MassTransitOptions 存在");
    var t1 = typeof(OrderCreatedConsumer);
    Console.WriteLine($"[PASS] OrderCreatedConsumer 存在");
    var t2 = typeof(OrderStateMachine);
    Console.WriteLine($"[PASS] OrderStateMachine 存在");
    var t3 = typeof(OrderState);
    Console.WriteLine($"[PASS] OrderState 存在");
    var t4 = typeof(OrderStateDbContext);
    Console.WriteLine($"[PASS] OrderStateDbContext 存在");
    var t5 = typeof(OrderStateMap);
    Console.WriteLine($"[PASS] OrderStateMap 存在");
    var t6 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t7 = typeof(MassTransitBackgroundService);
    Console.WriteLine($"[PASS] MassTransitBackgroundService 存在");
    var t8 = typeof(AdvancedMassTransitScenarios);
    Console.WriteLine($"[PASS] AdvancedMassTransitScenarios 存在");
    var t9 = typeof(OrderSaga);
    Console.WriteLine($"[PASS] OrderSaga 存在");
    var t10 = typeof(OrderCreatedEvent);
    Console.WriteLine($"[PASS] OrderCreatedEvent record 存在");
    var t11 = typeof(OrderProcessedEvent);
    Console.WriteLine($"[PASS] OrderProcessedEvent record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}