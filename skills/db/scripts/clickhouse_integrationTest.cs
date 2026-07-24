#load "clickhouse_integration.cs"

Console.WriteLine("=== clickhouse_integration Test ===");

try
{
    var t0 = typeof(ClickHouseIntegration.ClickHouseOptions);
    Console.WriteLine($"[PASS] ClickHouseOptions 存在");
    var t1 = typeof(ClickHouseIntegration.ClickHouseResult);
    Console.WriteLine($"[PASS] ClickHouseResult 存在");
    var t2 = typeof(ClickHouseIntegration.ClickHouseService);
    Console.WriteLine($"[PASS] ClickHouseService 存在");
    var t3 = typeof(ClickHouseIntegration.ClickHouseServiceExtensions);
    Console.WriteLine($"[PASS] ClickHouseServiceExtensions 存在");
    var t4 = typeof(ClickHouseIntegration.ClickHouseController);
    Console.WriteLine($"[PASS] ClickHouseController 存在");
    var t5 = typeof(ClickHouseIntegration.QueryRequest);
    Console.WriteLine($"[PASS] QueryRequest 存在");
    var t6 = typeof(ClickHouseIntegration.BulkInsertRequest);
    Console.WriteLine($"[PASS] BulkInsertRequest 存在");
    var t7 = typeof(ClickHouseIntegration.IClickHouseService);
    Console.WriteLine($"[PASS] IClickHouseService 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}