#load "sqlite_integration.cs"

Console.WriteLine("=== sqlite_integration Test ===");

try
{
    var t0 = typeof(SqliteIntegration.Customer);
    Console.WriteLine($"[PASS] Customer 存在");
    var t1 = typeof(SqliteIntegration.AppDbContext);
    Console.WriteLine($"[PASS] AppDbContext 存在");
    var t2 = typeof(SqliteIntegration.SqliteOptions);
    Console.WriteLine($"[PASS] SqliteOptions 存在");
    var t3 = typeof(SqliteIntegration.SqliteConnectionPool);
    Console.WriteLine($"[PASS] SqliteConnectionPool 存在");
    var t4 = typeof(SqliteIntegration.SqliteDatabaseService);
    Console.WriteLine($"[PASS] SqliteDatabaseService 存在");
    var t5 = typeof(SqliteIntegration.SqliteIntegrationExtensions);
    Console.WriteLine($"[PASS] SqliteIntegrationExtensions 存在");
    var t6 = typeof(SqliteIntegration.SqliteController);
    Console.WriteLine($"[PASS] SqliteController 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}