#load "natasha_integration.cs"

Console.WriteLine("=== natasha_integration Test ===");

try
{
    var t0 = typeof(Plugin.Natasha.NatashaIntegrationOptions);
    Console.WriteLine($"[PASS] NatashaIntegrationOptions 存在");
    var t1 = typeof(Plugin.Natasha.NatashaCodeGenerator);
    Console.WriteLine($"[PASS] NatashaCodeGenerator 存在");
    var t2 = typeof(Plugin.Natasha.DynamicPluginGenerator);
    Console.WriteLine($"[PASS] DynamicPluginGenerator 存在");
    var t3 = typeof(Plugin.Natasha.NatashaServiceCollectionExtensions);
    Console.WriteLine($"[PASS] NatashaServiceCollectionExtensions 存在");
    var t4 = typeof(Plugin.Natasha.IPlugin);
    Console.WriteLine($"[PASS] IPlugin 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}