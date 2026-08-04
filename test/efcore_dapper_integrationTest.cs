#load "efcore_dapper_integration.cs"

Console.WriteLine("=== efcore_dapper_integration Test ===");

try
{
    var t0 = typeof(AppDbContext);
    Console.WriteLine($"[PASS] AppDbContext 存在");
    var t1 = typeof(HashBasedDatabaseRouter);
    Console.WriteLine($"[PASS] HashBasedDatabaseRouter 存在");
    var t2 = typeof(OrderQueries);
    Console.WriteLine($"[PASS] OrderQueries 存在");
    var t3 = typeof(DateTimeOffsetHandler);
    Console.WriteLine($"[PASS] DateTimeOffsetHandler 存在");
    var t4 = typeof(ConsistentHashRouter);
    Console.WriteLine($"[PASS] ConsistentHashRouter 存在");
    var t5 = typeof(DapperBatchExtensions);
    Console.WriteLine($"[PASS] DapperBatchExtensions 存在");
    var t6 = typeof(Order);
    Console.WriteLine($"[PASS] Order record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}