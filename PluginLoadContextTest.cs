#load "PluginLoadContext.cs"

Console.WriteLine("=== PluginLoadContext Test ===");

try
{
    var t0 = typeof(PluginLoadContext);
    Console.WriteLine($"[PASS] PluginLoadContext 存在");
    var t1 = typeof(IsolatedPluginLoader);
    Console.WriteLine($"[PASS] IsolatedPluginLoader 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}