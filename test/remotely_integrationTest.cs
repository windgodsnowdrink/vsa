#load "remotely_integration.cs"

Console.WriteLine("=== remotely_integration Test ===");

try
{
    var t0 = typeof(Remotely.Integration.RemotelyOptions);
    Console.WriteLine($"[PASS] RemotelyOptions 存在");
    var t1 = typeof(Remotely.Integration.RemotelyService);
    Console.WriteLine($"[PASS] RemotelyService 存在");
    var t2 = typeof(Remotely.Integration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(Remotely.Integration.RemotelyHostedService);
    Console.WriteLine($"[PASS] RemotelyHostedService 存在");
    var t4 = typeof(Remotely.Integration.IRemotelyService);
    Console.WriteLine($"[PASS] IRemotelyService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}