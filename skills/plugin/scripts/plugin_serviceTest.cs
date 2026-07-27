#load "plugin_service.cs"

Console.WriteLine("=== plugin_service Test ===");

try
{
    var t0 = typeof(Plugin.Service.PluginServiceOptions);
    Console.WriteLine($"[PASS] PluginServiceOptions 存在");
    var t1 = typeof(Plugin.Service.PluginInfo);
    Console.WriteLine($"[PASS] PluginInfo 存在");
    var t2 = typeof(Plugin.Service.PluginEventArgs);
    Console.WriteLine($"[PASS] PluginEventArgs 存在");
    var t3 = typeof(Plugin.Service.PluginService);
    Console.WriteLine($"[PASS] PluginService 存在");
    var t4 = typeof(Plugin.Service.PluginServiceCollectionExtensions);
    Console.WriteLine($"[PASS] PluginServiceCollectionExtensions 存在");
    var t5 = typeof(Plugin.Service.PluginStatus);
    Console.WriteLine($"[PASS] PluginStatus enum 存在 (IsEnum: {t5.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}