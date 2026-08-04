#load "netservercore_integration.cs"

Console.WriteLine("=== netservercore_integration Test ===");

try
{
    var t0 = typeof(NetServerCoreOptions);
    Console.WriteLine($"[PASS] NetServerCoreOptions 存在");
    var t1 = typeof(NetServerCoreService);
    Console.WriteLine($"[PASS] NetServerCoreService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(INetServerCoreService);
    Console.WriteLine($"[PASS] INetServerCoreService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}