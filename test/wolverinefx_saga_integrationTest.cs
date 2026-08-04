#load "wolverinefx_saga_integration.cs"

Console.WriteLine("=== wolverinefx_saga_integration Test ===");

try
{
    var t0 = typeof(SagaState);
    Console.WriteLine($"[PASS] SagaState 存在");
    var t1 = typeof(SagaDbContext);
    Console.WriteLine($"[PASS] SagaDbContext 存在");
    var t2 = typeof(CreateOrderStep);
    Console.WriteLine($"[PASS] CreateOrderStep 存在");
    var t3 = typeof(SagaCoordinator);
    Console.WriteLine($"[PASS] SagaCoordinator 存在");
    var t4 = typeof(OrderProcessingSaga);
    Console.WriteLine($"[PASS] OrderProcessingSaga 存在");
    var t5 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t6 = typeof(ISagaStep);
    Console.WriteLine($"[PASS] ISagaStep 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(OrderCreated);
    Console.WriteLine($"[PASS] OrderCreated record 存在");
    var t8 = typeof(ProcessPaymentCommand);
    Console.WriteLine($"[PASS] ProcessPaymentCommand record 存在");
    var t9 = typeof(PaymentCompleted);
    Console.WriteLine($"[PASS] PaymentCompleted record 存在");
    var t10 = typeof(ReserveInventoryCommand);
    Console.WriteLine($"[PASS] ReserveInventoryCommand record 存在");
    var t11 = typeof(InventoryReserved);
    Console.WriteLine($"[PASS] InventoryReserved record 存在");
    var t12 = typeof(OrderFailed);
    Console.WriteLine($"[PASS] OrderFailed record 存在");
    var t13 = typeof(RefundPaymentCommand);
    Console.WriteLine($"[PASS] RefundPaymentCommand record 存在");
    var t14 = typeof(ReleaseInventoryCommand);
    Console.WriteLine($"[PASS] ReleaseInventoryCommand record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}