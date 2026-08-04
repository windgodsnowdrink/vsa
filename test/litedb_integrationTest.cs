#load "litedb_integration.cs"

Console.WriteLine("=== litedb_integration Test ===");

try
{
    var t0 = typeof(LiteDbOptions);
    Console.WriteLine($"[PASS] LiteDbOptions 存在");
    var t1 = typeof(LiteDbConnectionPool);
    Console.WriteLine($"[PASS] LiteDbConnectionPool 存在");
    var t2 = typeof(LiteDbServiceCollectionExtensions);
    Console.WriteLine($"[PASS] LiteDbServiceCollectionExtensions 存在");
    var t3 = typeof(LiteDbExampleService);
    Console.WriteLine($"[PASS] LiteDbExampleService 存在");
    var t4 = typeof(ExampleEntity);
    Console.WriteLine($"[PASS] ExampleEntity 存在");
    var t5 = typeof(LiteDbContext);
    Console.WriteLine($"[PASS] LiteDbContext 存在");
    var t6 = typeof(LiteDbBulkOperations);
    Console.WriteLine($"[PASS] LiteDbBulkOperations 存在");
    var t7 = typeof(ILiteDbConnectionPool);
    Console.WriteLine($"[PASS] ILiteDbConnectionPool 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}