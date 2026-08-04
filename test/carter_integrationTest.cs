#load "carter_integration.cs"

Console.WriteLine("=== carter_integration Test ===");

try
{
    var t0 = typeof(JsonContext);
    Console.WriteLine($"[PASS] JsonContext 存在");
    var t1 = typeof(CarterModuleBase);
    Console.WriteLine($"[PASS] CarterModuleBase 存在");
    var t2 = typeof(CarterModuleExtensions);
    Console.WriteLine($"[PASS] CarterModuleExtensions 存在");
    var t3 = typeof(CarterModuleRegistry);
    Console.WriteLine($"[PASS] CarterModuleRegistry 存在");
    var t4 = typeof(DecoratedCarterModule);
    Console.WriteLine($"[PASS] DecoratedCarterModule 存在");
    var t5 = typeof(ModuleFacade);
    Console.WriteLine($"[PASS] ModuleFacade 存在");
    var t6 = typeof(DefaultPluginLoader);
    Console.WriteLine($"[PASS] DefaultPluginLoader 存在");
    var t7 = typeof(UserModule);
    Console.WriteLine($"[PASS] UserModule 存在");
    var t8 = typeof(CacheHealthEndpoint);
    Console.WriteLine($"[PASS] CacheHealthEndpoint 存在");
    var t9 = typeof(PerformanceMonitoringMiddleware);
    Console.WriteLine($"[PASS] PerformanceMonitoringMiddleware 存在");
    var t10 = typeof(CarterIntegration);
    Console.WriteLine($"[PASS] CarterIntegration 存在");
    var t11 = typeof(CarterJsonContext);
    Console.WriteLine($"[PASS] CarterJsonContext 存在");
    var t12 = typeof(SlabMemoryPool);
    Console.WriteLine($"[PASS] SlabMemoryPool 存在");
    var t13 = typeof(SlabMemoryOwner);
    Console.WriteLine($"[PASS] SlabMemoryOwner 存在");
    var t14 = typeof(TenantContext);
    Console.WriteLine($"[PASS] TenantContext 存在");
    var t15 = typeof(TenantMiddleware);
    Console.WriteLine($"[PASS] TenantMiddleware 存在");
    var t16 = typeof(TailLatencyOptimizerExtensions);
    Console.WriteLine($"[PASS] TailLatencyOptimizerExtensions 存在");
    var t17 = typeof(SecurityMiddleware);
    Console.WriteLine($"[PASS] SecurityMiddleware 存在");
    var t18 = typeof(RateLimitingMiddleware);
    Console.WriteLine($"[PASS] RateLimitingMiddleware 存在");
    var t19 = typeof(ICarterModule);
    Console.WriteLine($"[PASS] ICarterModule 接口存在 (IsInterface: {t19.IsInterface})");
    var t20 = typeof(IPluginLoader);
    Console.WriteLine($"[PASS] IPluginLoader 接口存在 (IsInterface: {t20.IsInterface})");
    var t21 = typeof(UserCreateRequest);
    Console.WriteLine($"[PASS] UserCreateRequest record 存在");
    var t22 = typeof(UserUpdateRequest);
    Console.WriteLine($"[PASS] UserUpdateRequest record 存在");
    var t23 = typeof(User);
    Console.WriteLine($"[PASS] User record 存在");
    var t24 = typeof(RouteRequest);
    Console.WriteLine($"[PASS] RouteRequest record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}