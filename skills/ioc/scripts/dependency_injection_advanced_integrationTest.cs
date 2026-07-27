#load "dependency_injection_advanced_integration.cs"

Console.WriteLine("=== dependency_injection_advanced_integration Test ===");

try
{
    var t0 = typeof(Service);
    Console.WriteLine($"[PASS] Service 存在");
    var t1 = typeof(DecoratedService);
    Console.WriteLine($"[PASS] DecoratedService 存在");
    var t2 = typeof(Decorator);
    Console.WriteLine($"[PASS] Decorator 存在");
    var t3 = typeof(ScannedService);
    Console.WriteLine($"[PASS] ScannedService 存在");
    var t4 = typeof(FactoryService);
    Console.WriteLine($"[PASS] FactoryService 存在");
    var t5 = typeof(GenericService);
    Console.WriteLine($"[PASS] GenericService 存在");
    var t6 = typeof(NamedServiceA);
    Console.WriteLine($"[PASS] NamedServiceA 存在");
    var t7 = typeof(NamedServiceB);
    Console.WriteLine($"[PASS] NamedServiceB 存在");
    var t8 = typeof(ValidatedService);
    Console.WriteLine($"[PASS] ValidatedService 存在");
    var t9 = typeof(LifetimeEventsService);
    Console.WriteLine($"[PASS] LifetimeEventsService 存在");
    var t10 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t11 = typeof(IService);
    Console.WriteLine($"[PASS] IService 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(IDecoratedService);
    Console.WriteLine($"[PASS] IDecoratedService 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(IScannedService);
    Console.WriteLine($"[PASS] IScannedService 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(IFactoryService);
    Console.WriteLine($"[PASS] IFactoryService 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(IGenericService);
    Console.WriteLine($"[PASS] IGenericService 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(INamedService);
    Console.WriteLine($"[PASS] INamedService 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(IValidatedService);
    Console.WriteLine($"[PASS] IValidatedService 接口存在 (IsInterface: {t17.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}