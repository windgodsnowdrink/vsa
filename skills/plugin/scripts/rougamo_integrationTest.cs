#load "rougamo_integration.cs"

Console.WriteLine("=== rougamo_integration Test ===");

try
{
    var t0 = typeof(Plugin.AOP.PluginInterceptorAttribute);
    Console.WriteLine($"[PASS] PluginInterceptorAttribute 存在");
    var t1 = typeof(Plugin.AOP.PerformanceMonitorAttribute);
    Console.WriteLine($"[PASS] PerformanceMonitorAttribute 存在");
    var t2 = typeof(Plugin.AOP.TransactionAttribute);
    Console.WriteLine($"[PASS] TransactionAttribute 存在");
    var t3 = typeof(Plugin.AOP.CacheAttribute);
    Console.WriteLine($"[PASS] CacheAttribute 存在");
    var t4 = typeof(Plugin.AOP.RougamoIntegrationService);
    Console.WriteLine($"[PASS] RougamoIntegrationService 存在");
    var t5 = typeof(Plugin.AOP.RougamoDependencyInjection);
    Console.WriteLine($"[PASS] RougamoDependencyInjection 存在");
    var t6 = typeof(Plugin.AOP.SamplePlugin);
    Console.WriteLine($"[PASS] SamplePlugin 存在");
    var t7 = typeof(Plugin.AOP.IPlugin);
    Console.WriteLine($"[PASS] IPlugin 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}