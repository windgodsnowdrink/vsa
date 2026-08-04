#load "unitofwork_integration.cs"

Console.WriteLine("=== unitofwork_integration Test ===");

try
{
    var t0 = typeof(UnitOfWork);
    Console.WriteLine($"[PASS] UnitOfWork 存在");
    var t1 = typeof(Repository);
    Console.WriteLine($"[PASS] Repository 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(RepositoryPoolPolicy);
    Console.WriteLine($"[PASS] RepositoryPoolPolicy 存在");
    var t4 = typeof(EnhancedRepository);
    Console.WriteLine($"[PASS] EnhancedRepository 存在");
    var t5 = typeof(EnhancedUnitOfWork);
    Console.WriteLine($"[PASS] EnhancedUnitOfWork 存在");
    var t6 = typeof(IUnitOfWork);
    Console.WriteLine($"[PASS] IUnitOfWork 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(IRepository);
    Console.WriteLine($"[PASS] IRepository 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(IAggregateRoot);
    Console.WriteLine($"[PASS] IAggregateRoot 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(IBulkRepository);
    Console.WriteLine($"[PASS] IBulkRepository 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(IAnalyticalRepository);
    Console.WriteLine($"[PASS] IAnalyticalRepository 接口存在 (IsInterface: {t10.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}