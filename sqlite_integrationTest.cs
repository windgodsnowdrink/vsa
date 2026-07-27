#load "sqlite_integration.cs"

Console.WriteLine("=== sqlite_integration Test ===");

try
{
    var t0 = typeof(SqliteIntegration.AppDbContext);
    Console.WriteLine($"[PASS] AppDbContext 存在");
    var t1 = typeof(SqliteIntegration.SqliteOptions);
    Console.WriteLine($"[PASS] SqliteOptions 存在");
    var t2 = typeof(SqliteIntegration.SqliteConnectionPool);
    Console.WriteLine($"[PASS] SqliteConnectionPool 存在");
    var t3 = typeof(SqliteIntegration.SqliteServiceExtensions);
    Console.WriteLine($"[PASS] SqliteServiceExtensions 存在");
    var t4 = typeof(SqliteIntegration.SqliteTransactionScope);
    Console.WriteLine($"[PASS] SqliteTransactionScope 存在");
    var t5 = typeof(SqliteIntegration.SqliteBulkInserter);
    Console.WriteLine($"[PASS] SqliteBulkInserter 存在");
    var t6 = typeof(SqliteIntegration.EfCoreBulkOperations);
    Console.WriteLine($"[PASS] EfCoreBulkOperations 存在");
    var t7 = typeof(SqliteIntegration.SqliteDbExampleService);
    Console.WriteLine($"[PASS] SqliteDbExampleService 存在");
    var t8 = typeof(SqliteIntegration.Customer);
    Console.WriteLine($"[PASS] Customer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}