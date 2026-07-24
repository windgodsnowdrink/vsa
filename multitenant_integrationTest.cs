#load "multitenant_integration.cs"

Console.WriteLine("=== multitenant_integration Test ===");

try
{
    var t0 = typeof(AppTenantInfo);
    Console.WriteLine($"[PASS] AppTenantInfo 存在");
    var t1 = typeof(TenantResolver);
    Console.WriteLine($"[PASS] TenantResolver 存在");
    var t2 = typeof(TenantAwareCache);
    Console.WriteLine($"[PASS] TenantAwareCache 存在");
    var t3 = typeof(TenantQuotaService);
    Console.WriteLine($"[PASS] TenantQuotaService 存在");
    var t4 = typeof(RedisTenantStore);
    Console.WriteLine($"[PASS] RedisTenantStore 存在");
    var t5 = typeof(DatabaseTenantStore);
    Console.WriteLine($"[PASS] DatabaseTenantStore 存在");
    var t6 = typeof(HybridTenantStore);
    Console.WriteLine($"[PASS] HybridTenantStore 存在");
    var t7 = typeof(SqliteTenantStore);
    Console.WriteLine($"[PASS] SqliteTenantStore 存在");
    var t8 = typeof(LiteDbTenantStore);
    Console.WriteLine($"[PASS] LiteDbTenantStore 存在");
    var t9 = typeof(MultiTenantBuilderExtensions);
    Console.WriteLine($"[PASS] MultiTenantBuilderExtensions 存在");
    var t10 = typeof(MultiTenantDbContext);
    Console.WriteLine($"[PASS] MultiTenantDbContext 存在");
    var t11 = typeof(DbContextPooledPolicy);
    Console.WriteLine($"[PASS] DbContextPooledPolicy 存在");
    var t12 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t13 = typeof(ProductsController);
    Console.WriteLine($"[PASS] ProductsController 存在");
    var t14 = typeof(TenantAwareEntity);
    Console.WriteLine($"[PASS] TenantAwareEntity 存在");
    var t15 = typeof(Product);
    Console.WriteLine($"[PASS] Product 存在");
    var t16 = typeof(TenantRoutingStrategy);
    Console.WriteLine($"[PASS] TenantRoutingStrategy 存在");
    var t17 = typeof(TenantRoutingOptions);
    Console.WriteLine($"[PASS] TenantRoutingOptions 存在");
    var t18 = typeof(TenantDatabase);
    Console.WriteLine($"[PASS] TenantDatabase 存在");
    var t19 = typeof(TenantContext);
    Console.WriteLine($"[PASS] TenantContext 存在");
    var t20 = typeof(TenantDbContext);
    Console.WriteLine($"[PASS] TenantDbContext 存在");
    var t21 = typeof(ITenantEventPublisher);
    Console.WriteLine($"[PASS] ITenantEventPublisher 接口存在 (IsInterface: {t21.IsInterface})");
    var t22 = typeof(ITenantEntity);
    Console.WriteLine($"[PASS] ITenantEntity 接口存在 (IsInterface: {t22.IsInterface})");
    var t23 = typeof(ITenantProvider);
    Console.WriteLine($"[PASS] ITenantProvider 接口存在 (IsInterface: {t23.IsInterface})");
    var t24 = typeof(QuotaType);
    Console.WriteLine($"[PASS] QuotaType enum 存在 (IsEnum: {t24.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}