#load "wolverinefx_orleans_integration.cs"

Console.WriteLine("=== wolverinefx_orleans_integration Test ===");

try
{
    var t0 = typeof(WolverineGrain);
    Console.WriteLine($"[PASS] WolverineGrain 存在");
    var t1 = typeof(WolverineGrainState);
    Console.WriteLine($"[PASS] WolverineGrainState 存在");
    var t2 = typeof(OrleansMessageHandler);
    Console.WriteLine($"[PASS] OrleansMessageHandler 存在");
    var t3 = typeof(WolverineOrleansIntegrationExtensions);
    Console.WriteLine($"[PASS] WolverineOrleansIntegrationExtensions 存在");
    var t4 = typeof(OrleansTransport);
    Console.WriteLine($"[PASS] OrleansTransport 存在");
    var t5 = typeof(IWolverineGrain);
    Console.WriteLine($"[PASS] IWolverineGrain 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(CrossGrainMessage);
    Console.WriteLine($"[PASS] CrossGrainMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}