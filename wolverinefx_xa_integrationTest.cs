#load "wolverinefx_xa_integration.cs"

Console.WriteLine("=== wolverinefx_xa_integration Test ===");

try
{
    var t0 = typeof(XATransactionMiddleware);
    Console.WriteLine($"[PASS] XATransactionMiddleware 存在");
    var t1 = typeof(WolverineDependencyInjectionExtensions);
    Console.WriteLine($"[PASS] WolverineDependencyInjectionExtensions 存在");
    var t2 = typeof(OrderService);
    Console.WriteLine($"[PASS] OrderService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}