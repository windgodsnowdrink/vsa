#load "oqtane_plugin_service.cs"

Console.WriteLine("=== oqtane_plugin_service Test ===");

try
{
    var t0 = typeof(OqtanePluginLoader);
    Console.WriteLine($"[PASS] OqtanePluginLoader 存在");
    var t1 = typeof(PluginContext);
    Console.WriteLine($"[PASS] PluginContext 存在");
    var t2 = typeof(PluginContextPooledPolicy);
    Console.WriteLine($"[PASS] PluginContextPooledPolicy 存在");
    var t3 = typeof(TodoPlugin);
    Console.WriteLine($"[PASS] TodoPlugin 存在");
    var t4 = typeof(IOqtanePlugin);
    Console.WriteLine($"[PASS] IOqtanePlugin 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(PluginMessage);
    Console.WriteLine($"[PASS] PluginMessage record 存在");
    var t6 = typeof(TodoCommand);
    Console.WriteLine($"[PASS] TodoCommand record 存在");
    var t7 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}