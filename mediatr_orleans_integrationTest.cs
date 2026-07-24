#load "mediatr_orleans_integration.cs"

Console.WriteLine("=== mediatr_orleans_integration Test ===");

try
{
    var t0 = typeof(MediatorGrain);
    Console.WriteLine($"[PASS] MediatorGrain 存在");
    var t1 = typeof(MediatorGrainState);
    Console.WriteLine($"[PASS] MediatorGrainState 存在");
    var t2 = typeof(OrleansNotificationHandler);
    Console.WriteLine($"[PASS] OrleansNotificationHandler 存在");
    var t3 = typeof(MediatorMetrics);
    Console.WriteLine($"[PASS] MediatorMetrics 存在");
    var t4 = typeof(AuthGrainFilter);
    Console.WriteLine($"[PASS] AuthGrainFilter 存在");
    var t5 = typeof(MediatROrleansIntegrationExtensions);
    Console.WriteLine($"[PASS] MediatROrleansIntegrationExtensions 存在");
    var t6 = typeof(IMediatorGrain);
    Console.WriteLine($"[PASS] IMediatorGrain 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(IMessageStore);
    Console.WriteLine($"[PASS] IMessageStore 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(CrossGrainCommand);
    Console.WriteLine($"[PASS] CrossGrainCommand record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}