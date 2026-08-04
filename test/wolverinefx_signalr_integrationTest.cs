#load "wolverinefx_signalr_integration.cs"

Console.WriteLine("=== wolverinefx_signalr_integration Test ===");

try
{
    var t0 = typeof(WolverineSignalRIntegrationExtensions);
    Console.WriteLine($"[PASS] WolverineSignalRIntegrationExtensions 存在");
    var t1 = typeof(RealtimeQueryService);
    Console.WriteLine($"[PASS] RealtimeQueryService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}