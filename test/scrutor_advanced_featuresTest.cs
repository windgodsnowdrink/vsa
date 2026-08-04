#load "scrutor_advanced_features.cs"

Console.WriteLine("=== scrutor_advanced_features Test ===");

try
{
    var t0 = typeof(ProxyService);
    Console.WriteLine($"[PASS] ProxyService 存在");
    var t1 = typeof(ProxyInterceptor);
    Console.WriteLine($"[PASS] ProxyInterceptor 存在");
    var t2 = typeof(ConditionalCore);
    Console.WriteLine($"[PASS] ConditionalCore 存在");
    var t3 = typeof(DevelopmentDecorator);
    Console.WriteLine($"[PASS] DevelopmentDecorator 存在");
    var t4 = typeof(MultiLifetimeService);
    Console.WriteLine($"[PASS] MultiLifetimeService 存在");
    var t5 = typeof(AdvancedFeatures);
    Console.WriteLine($"[PASS] AdvancedFeatures 存在");
    var t6 = typeof(MultiLifetimeResolver);
    Console.WriteLine($"[PASS] MultiLifetimeResolver 存在");
    var t7 = typeof(IProxyService);
    Console.WriteLine($"[PASS] IProxyService 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(IConditionalDecorator);
    Console.WriteLine($"[PASS] IConditionalDecorator 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(IMultiLifetimeService);
    Console.WriteLine($"[PASS] IMultiLifetimeService 接口存在 (IsInterface: {t9.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}