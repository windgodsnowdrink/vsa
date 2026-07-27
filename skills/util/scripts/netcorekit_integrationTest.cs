#load "netcorekit_integration.cs"

Console.WriteLine("=== netcorekit_integration Test ===");

try
{
    var t0 = typeof(NetCoreKitExtensions.NetCoreKitOptions);
    Console.WriteLine($"[PASS] NetCoreKitOptions 存在");
    var t1 = typeof(NetCoreKitExtensions.NetCoreKitService);
    Console.WriteLine($"[PASS] NetCoreKitService 存在");
    var t2 = typeof(NetCoreKitExtensions.NetCoreKitServiceCollectionExtensions);
    Console.WriteLine($"[PASS] NetCoreKitServiceCollectionExtensions 存在");
    var t3 = typeof(NetCoreKitExtensions.ExampleUsage);
    Console.WriteLine($"[PASS] ExampleUsage 存在");
    var t4 = typeof(NetCoreKitExtensions.INetCoreKitService);
    Console.WriteLine($"[PASS] INetCoreKitService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}