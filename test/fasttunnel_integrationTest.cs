#load "fasttunnel_integration.cs"

Console.WriteLine("=== fasttunnel_integration Test ===");

try
{
    var t0 = typeof(FastTunnelIntegration.FastTunnelOptions);
    Console.WriteLine($"[PASS] FastTunnelOptions 存在");
    var t1 = typeof(FastTunnelIntegration.FastTunnelService);
    Console.WriteLine($"[PASS] FastTunnelService 存在");
    var t2 = typeof(FastTunnelIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(FastTunnelIntegration.ForwardInfo);
    Console.WriteLine($"[PASS] ForwardInfo 存在");
    var t4 = typeof(FastTunnelIntegration.ServerInfo);
    Console.WriteLine($"[PASS] ServerInfo 存在");
    var t5 = typeof(FastTunnelIntegration.IFastTunnelService);
    Console.WriteLine($"[PASS] IFastTunnelService 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}