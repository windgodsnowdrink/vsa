#load "nacos_integration.cs"

Console.WriteLine("=== nacos_integration Test ===");

try
{
    var t0 = typeof(NacosIntegration.NacosOptions);
    Console.WriteLine($"[PASS] NacosOptions 存在");
    var t1 = typeof(NacosIntegration.NacosService);
    Console.WriteLine($"[PASS] NacosService 存在");
    var t2 = typeof(NacosIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(NacosIntegration.INacosService);
    Console.WriteLine($"[PASS] INacosService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}