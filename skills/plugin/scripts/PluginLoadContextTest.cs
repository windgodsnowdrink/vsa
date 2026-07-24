#load "PluginLoadContext.cs"

Console.WriteLine("=== PluginLoadContext Test ===");

try
{
    var t0 = typeof(Plugin.PluginLoadOptions);
    Console.WriteLine($"[PASS] PluginLoadOptions 存在");
    var t1 = typeof(Plugin.PluginLoadContext);
    Console.WriteLine($"[PASS] PluginLoadContext 存在");
    var t2 = typeof(Plugin.IsolatedPluginLoader);
    Console.WriteLine($"[PASS] IsolatedPluginLoader 存在");
    var t3 = typeof(Plugin.PluginLoaderFactory);
    Console.WriteLine($"[PASS] PluginLoaderFactory 存在");
    var t4 = typeof(Plugin.PluginServiceCollectionExtensions);
    Console.WriteLine($"[PASS] PluginServiceCollectionExtensions 存在");
    var t5 = typeof(Plugin.IPluginLoader);
    Console.WriteLine($"[PASS] IPluginLoader 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}