#load "mediatr_signalr_integration.cs"

Console.WriteLine("=== mediatr_signalr_integration Test ===");

try
{
    var t0 = typeof(SignalRNotificationHandler);
    Console.WriteLine($"[PASS] SignalRNotificationHandler 存在");
    var t1 = typeof(MediatRSignalRIntegrationExtensions);
    Console.WriteLine($"[PASS] MediatRSignalRIntegrationExtensions 存在");
    var t2 = typeof(RealtimeQueryService);
    Console.WriteLine($"[PASS] RealtimeQueryService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}