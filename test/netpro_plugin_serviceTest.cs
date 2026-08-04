#load "netpro_plugin_service.cs"

Console.WriteLine("=== netpro_plugin_service Test ===");

try
{
    var t0 = typeof(PluginEngine);
    Console.WriteLine($"[PASS] PluginEngine 存在");
    var t1 = typeof(PluginContext);
    Console.WriteLine($"[PASS] PluginContext 存在");
    var t2 = typeof(PluginContextPooledPolicy);
    Console.WriteLine($"[PASS] PluginContextPooledPolicy 存在");
    var t3 = typeof(TodoPlugin);
    Console.WriteLine($"[PASS] TodoPlugin 存在");
    var t4 = typeof(INetProPlugin);
    Console.WriteLine($"[PASS] INetProPlugin 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(PluginMessage);
    Console.WriteLine($"[PASS] PluginMessage record 存在");
    var t6 = typeof(PluginResult);
    Console.WriteLine($"[PASS] PluginResult record 存在");
    var t7 = typeof(TodoCommand);
    Console.WriteLine($"[PASS] TodoCommand record 存在");
    var t8 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}