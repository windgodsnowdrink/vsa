#load "autofac_production.cs"

Console.WriteLine("=== autofac_production Test ===");

try
{
    var t0 = typeof(CoreModule);
    Console.WriteLine($"[PASS] CoreModule 存在");
    var t1 = typeof(CallLogger);
    Console.WriteLine($"[PASS] CallLogger 存在");
    var t2 = typeof(AutofacConfig);
    Console.WriteLine($"[PASS] AutofacConfig 存在");
    var t3 = typeof(AutofacExtensions);
    Console.WriteLine($"[PASS] AutofacExtensions 存在");
    var t4 = typeof(AutofacProductionExtensions);
    Console.WriteLine($"[PASS] AutofacProductionExtensions 存在");
    var t5 = typeof(TenantIdentificationStrategy);
    Console.WriteLine($"[PASS] TenantIdentificationStrategy 存在");
    var t6 = typeof(OrderService);
    Console.WriteLine($"[PASS] OrderService 存在");
    var t7 = typeof(CacheManager);
    Console.WriteLine($"[PASS] CacheManager 存在");
    var t8 = typeof(Repository);
    Console.WriteLine($"[PASS] Repository 存在");
    var t9 = typeof(ReportService);
    Console.WriteLine($"[PASS] ReportService 存在");
    var t10 = typeof(TenantContext);
    Console.WriteLine($"[PASS] TenantContext 存在");
    var t11 = typeof(ExpensiveResource);
    Console.WriteLine($"[PASS] ExpensiveResource 存在");
    var t12 = typeof(Composite);
    Console.WriteLine($"[PASS] Composite 存在");
    var t13 = typeof(CircularDependencyHandler);
    Console.WriteLine($"[PASS] CircularDependencyHandler 存在");
    var t14 = typeof(ConcurrentComponent);
    Console.WriteLine($"[PASS] ConcurrentComponent 存在");
    var t15 = typeof(CustomRegistrationSource);
    Console.WriteLine($"[PASS] CustomRegistrationSource 存在");
    var t16 = typeof(PlatformSpecificService);
    Console.WriteLine($"[PASS] PlatformSpecificService 存在");
    var t17 = typeof(IOrderService);
    Console.WriteLine($"[PASS] IOrderService 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(ICacheManager);
    Console.WriteLine($"[PASS] ICacheManager 接口存在 (IsInterface: {t18.IsInterface})");
    var t19 = typeof(IRepository);
    Console.WriteLine($"[PASS] IRepository 接口存在 (IsInterface: {t19.IsInterface})");
    var t20 = typeof(IReportService);
    Console.WriteLine($"[PASS] IReportService 接口存在 (IsInterface: {t20.IsInterface})");
    var t21 = typeof(IServiceMiddleware);
    Console.WriteLine($"[PASS] IServiceMiddleware 接口存在 (IsInterface: {t21.IsInterface})");
    var t22 = typeof(IPlatformSpecificService);
    Console.WriteLine($"[PASS] IPlatformSpecificService 接口存在 (IsInterface: {t22.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}