#load "plugin_service.cs"

Console.WriteLine("=== plugin_service Test ===");

try
{
    var t0 = typeof(PluginLoader);
    Console.WriteLine($"[PASS] PluginLoader 存在");
    var t1 = typeof(PluginContext);
    Console.WriteLine($"[PASS] PluginContext 存在");
    var t2 = typeof(PluginContextPooledPolicy);
    Console.WriteLine($"[PASS] PluginContextPooledPolicy 存在");
    var t3 = typeof(IHotPlugModule);
    Console.WriteLine($"[PASS] IHotPlugModule 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(PluginMessage);
    Console.WriteLine($"[PASS] PluginMessage record 存在");
    var t5 = typeof(PluginLoaderOptions);
    Console.WriteLine($"[PASS] PluginLoaderOptions record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}