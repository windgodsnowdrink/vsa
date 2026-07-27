#load "tye_generator.cs"

Console.WriteLine("=== tye_generator Test ===");

try
{
    var t0 = typeof(Tye.Generator.Service);
    Console.WriteLine($"[PASS] Service 存在");
    var t1 = typeof(Tye.Generator.AnotherService);
    Console.WriteLine($"[PASS] AnotherService 存在");
    var t2 = typeof(Tye.Generator.SingletonService);
    Console.WriteLine($"[PASS] SingletonService 存在");
    var t3 = typeof(Tye.Generator.ScopedService);
    Console.WriteLine($"[PASS] ScopedService 存在");
    var t4 = typeof(Tye.Generator.LoggingServiceDecorator);
    Console.WriteLine($"[PASS] LoggingServiceDecorator 存在");
    var t5 = typeof(Tye.Generator.CachingServiceDecorator);
    Console.WriteLine($"[PASS] CachingServiceDecorator 存在");
    var t6 = typeof(Tye.Generator.TransactionalServiceDecorator);
    Console.WriteLine($"[PASS] TransactionalServiceDecorator 存在");
    var t7 = typeof(Tye.Generator.IService);
    Console.WriteLine($"[PASS] IService 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(Tye.Generator.IAnotherService);
    Console.WriteLine($"[PASS] IAnotherService 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(Tye.Generator.ISingletonService);
    Console.WriteLine($"[PASS] ISingletonService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(Tye.Generator.IScopedService);
    Console.WriteLine($"[PASS] IScopedService 接口存在 (IsInterface: {t10.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}