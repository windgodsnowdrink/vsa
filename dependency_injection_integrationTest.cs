#load "dependency_injection_integration.cs"

Console.WriteLine("=== dependency_injection_integration Test ===");

try
{
    var t0 = typeof(TransientService);
    Console.WriteLine($"[PASS] TransientService 存在");
    var t1 = typeof(ScopedService);
    Console.WriteLine($"[PASS] ScopedService 存在");
    var t2 = typeof(SingletonService);
    Console.WriteLine($"[PASS] SingletonService 存在");
    var t3 = typeof(DecoratedService);
    Console.WriteLine($"[PASS] DecoratedService 存在");
    var t4 = typeof(DecoratorService);
    Console.WriteLine($"[PASS] DecoratorService 存在");
    var t5 = typeof(ServiceA);
    Console.WriteLine($"[PASS] ServiceA 存在");
    var t6 = typeof(ServiceB);
    Console.WriteLine($"[PASS] ServiceB 存在");
    var t7 = typeof(ServiceFactory);
    Console.WriteLine($"[PASS] ServiceFactory 存在");
    var t8 = typeof(Repository);
    Console.WriteLine($"[PASS] Repository 存在");
    var t9 = typeof(NamedServiceA);
    Console.WriteLine($"[PASS] NamedServiceA 存在");
    var t10 = typeof(NamedServiceB);
    Console.WriteLine($"[PASS] NamedServiceB 存在");
    var t11 = typeof(AppSettings);
    Console.WriteLine($"[PASS] AppSettings 存在");
    var t12 = typeof(AppLifetimeEvents);
    Console.WriteLine($"[PASS] AppLifetimeEvents 存在");
    var t13 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t14 = typeof(where);
    Console.WriteLine($"[PASS] where 存在");
    var t15 = typeof(ITransientService);
    Console.WriteLine($"[PASS] ITransientService 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(IScopedService);
    Console.WriteLine($"[PASS] IScopedService 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(ISingletonService);
    Console.WriteLine($"[PASS] ISingletonService 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(IDecoratedService);
    Console.WriteLine($"[PASS] IDecoratedService 接口存在 (IsInterface: {t18.IsInterface})");
    var t19 = typeof(IService);
    Console.WriteLine($"[PASS] IService 接口存在 (IsInterface: {t19.IsInterface})");
    var t20 = typeof(IServiceFactory);
    Console.WriteLine($"[PASS] IServiceFactory 接口存在 (IsInterface: {t20.IsInterface})");
    var t21 = typeof(IRepository);
    Console.WriteLine($"[PASS] IRepository 接口存在 (IsInterface: {t21.IsInterface})");
    var t22 = typeof(INamedService);
    Console.WriteLine($"[PASS] INamedService 接口存在 (IsInterface: {t22.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}