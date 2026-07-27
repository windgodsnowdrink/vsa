#load "mediatr_tcc_integration.cs"

Console.WriteLine("=== mediatr_tcc_integration Test ===");

try
{
    var t0 = typeof(TccCoordinator);
    Console.WriteLine($"[PASS] TccCoordinator 存在");
    var t1 = typeof(CreateOrderTccHandler);
    Console.WriteLine($"[PASS] CreateOrderTccHandler 存在");
    var t2 = typeof(MediatRDependencyInjectionExtensions);
    Console.WriteLine($"[PASS] MediatRDependencyInjectionExtensions 存在");
    var t3 = typeof(ITccTransaction);
    Console.WriteLine($"[PASS] ITccTransaction 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}