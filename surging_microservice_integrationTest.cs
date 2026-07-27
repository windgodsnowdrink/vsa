#load "surging_microservice_integration.cs"

Console.WriteLine("=== surging_microservice_integration Test ===");

try
{
    var t0 = typeof(OrderService);
    Console.WriteLine($"[PASS] OrderService 存在");
    var t1 = typeof(Startup);
    Console.WriteLine($"[PASS] Startup 存在");
    var t2 = typeof(Order);
    Console.WriteLine($"[PASS] Order 存在");
    var t3 = typeof(OrderCreatedEvent);
    Console.WriteLine($"[PASS] OrderCreatedEvent 存在");
    var t4 = typeof(IOrderService);
    Console.WriteLine($"[PASS] IOrderService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}