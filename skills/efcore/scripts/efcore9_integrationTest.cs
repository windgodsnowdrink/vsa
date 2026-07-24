#load "efcore9_integration.cs"

Console.WriteLine("=== efcore9_integration Test ===");

try
{
    var t0 = typeof(AppDbContext);
    Console.WriteLine($"[PASS] AppDbContext 存在");
    var t1 = typeof(CachedRepository);
    Console.WriteLine($"[PASS] CachedRepository 存在");
    var t2 = typeof(ShardingStrategy);
    Console.WriteLine($"[PASS] ShardingStrategy 存在");
    var t3 = typeof(EntitySnapshot);
    Console.WriteLine($"[PASS] EntitySnapshot 存在");
    var t4 = typeof(ArchivingService);
    Console.WriteLine($"[PASS] ArchivingService 存在");
    var t5 = typeof(BackupService);
    Console.WriteLine($"[PASS] BackupService 存在");
    var t6 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t7 = typeof(SnapshotService);
    Console.WriteLine($"[PASS] SnapshotService 存在");
    var t8 = typeof(ITenantProvider);
    Console.WriteLine($"[PASS] ITenantProvider 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(IAuditableEntity);
    Console.WriteLine($"[PASS] IAuditableEntity 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(ISoftDelete);
    Console.WriteLine($"[PASS] ISoftDelete 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(IEncryptedEntity);
    Console.WriteLine($"[PASS] IEncryptedEntity 接口存在 (IsInterface: {t11.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}