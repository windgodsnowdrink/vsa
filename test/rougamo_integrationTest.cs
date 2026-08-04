#load "rougamo_integration.cs"

Console.WriteLine("=== rougamo_integration Test ===");

try
{
    var t0 = typeof(PluginAssemblyLoadContext);
    Console.WriteLine($"[PASS] PluginAssemblyLoadContext 存在");
    var t1 = typeof(PluginInterceptorAttribute);
    Console.WriteLine($"[PASS] PluginInterceptorAttribute 存在");
    var t2 = typeof(PluginManagerService);
    Console.WriteLine($"[PASS] PluginManagerService 存在");
    var t3 = typeof(PluginContextPooledPolicy);
    Console.WriteLine($"[PASS] PluginContextPooledPolicy 存在");
    var t4 = typeof(PluginContext);
    Console.WriteLine($"[PASS] PluginContext 存在");
    var t5 = typeof(PluginUnloadedException);
    Console.WriteLine($"[PASS] PluginUnloadedException 存在");
    var t6 = typeof(CalePlugin);
    Console.WriteLine($"[PASS] CalePlugin 存在");
    var t7 = typeof(PluginLoadExtensions);
    Console.WriteLine($"[PASS] PluginLoadExtensions 存在");
    var t8 = typeof(IPluginUnloadable);
    Console.WriteLine($"[PASS] IPluginUnloadable 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(PluginMetadata);
    Console.WriteLine($"[PASS] PluginMetadata record 存在");
    var t10 = typeof(PluginCommand);
    Console.WriteLine($"[PASS] PluginCommand record 存在");
    var t11 = typeof(LoadPluginCommand);
    Console.WriteLine($"[PASS] LoadPluginCommand record 存在");
    var t12 = typeof(UnloadPluginCommand);
    Console.WriteLine($"[PASS] UnloadPluginCommand record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}