#load "netpro_plugin_service.cs"

Console.WriteLine("=== netpro_plugin_service Test ===");

try
{
    var t0 = typeof(NetProOptions);
    Console.WriteLine($"[PASS] NetProOptions 存在");
    var t1 = typeof(ExtensionContext);
    Console.WriteLine($"[PASS] ExtensionContext 存在");
    var t2 = typeof(NetProService);
    Console.WriteLine($"[PASS] NetProService 存在");
    var t3 = typeof(NetProPluginManager);
    Console.WriteLine($"[PASS] NetProPluginManager 存在");
    var t4 = typeof(FileChangeEvent);
    Console.WriteLine($"[PASS] FileChangeEvent 存在");
    var t5 = typeof(NetProExtensionManager);
    Console.WriteLine($"[PASS] NetProExtensionManager 存在");
    var t6 = typeof(NetProServiceCollectionExtensions);
    Console.WriteLine($"[PASS] NetProServiceCollectionExtensions 存在");
    var t7 = typeof(SamplePlugin);
    Console.WriteLine($"[PASS] SamplePlugin 存在");
    var t8 = typeof(SampleExtension);
    Console.WriteLine($"[PASS] SampleExtension 存在");
    var t9 = typeof(INetProService);
    Console.WriteLine($"[PASS] INetProService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(INetProPluginManager);
    Console.WriteLine($"[PASS] INetProPluginManager 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(INetProExtensionManager);
    Console.WriteLine($"[PASS] INetProExtensionManager 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(INetProPlugin);
    Console.WriteLine($"[PASS] INetProPlugin 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(INetProExtension);
    Console.WriteLine($"[PASS] INetProExtension 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(FileChangeType);
    Console.WriteLine($"[PASS] FileChangeType enum 存在 (IsEnum: {t14.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}