#load "mediatr_xa_integration.cs"

Console.WriteLine("=== mediatr_xa_integration Test ===");

try
{
    var t0 = typeof(XATransactionBehavior);
    Console.WriteLine($"[PASS] XATransactionBehavior 存在");
    var t1 = typeof(MediatRDependencyInjectionExtensions);
    Console.WriteLine($"[PASS] MediatRDependencyInjectionExtensions 存在");
    var t2 = typeof(OrderService);
    Console.WriteLine($"[PASS] OrderService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}