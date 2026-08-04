#load "CastleDynamicProxy.cs"

Console.WriteLine("=== CastleDynamicProxy Test ===");

try
{
    var t0 = typeof(LoggingAttribute);
    Console.WriteLine($"[PASS] LoggingAttribute 存在");
    var t1 = typeof(ControlService);
    Console.WriteLine($"[PASS] ControlService 存在");
    var t2 = typeof(LoggingInterceptor);
    Console.WriteLine($"[PASS] LoggingInterceptor 存在");
    var t3 = typeof(ProxyFactory);
    Console.WriteLine($"[PASS] ProxyFactory 存在");
    var t4 = typeof(IService);
    Console.WriteLine($"[PASS] IService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}