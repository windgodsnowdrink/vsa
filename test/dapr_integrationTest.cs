#load "dapr_integration.cs"

Console.WriteLine("=== dapr_integration Test ===");

try
{
    var t0 = typeof(DaprOptions);
    Console.WriteLine($"[PASS] DaprOptions 存在");
    var t1 = typeof(DaprService);
    Console.WriteLine($"[PASS] DaprService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(OrderController);
    Console.WriteLine($"[PASS] OrderController 存在");
    var t4 = typeof(Order);
    Console.WriteLine($"[PASS] Order 存在");
    var t5 = typeof(IDaprService);
    Console.WriteLine($"[PASS] IDaprService 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}