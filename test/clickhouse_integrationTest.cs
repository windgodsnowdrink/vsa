#load "clickhouse_integration.cs"

Console.WriteLine("=== clickhouse_integration Test ===");

try
{
    var t0 = typeof(ClickHouseIntegration.ClickHouseOptions);
    Console.WriteLine($"[PASS] ClickHouseOptions 存在");
    var t1 = typeof(ClickHouseIntegration.ClickHouseService);
    Console.WriteLine($"[PASS] ClickHouseService 存在");
    var t2 = typeof(ClickHouseIntegration.BulkInsertItem);
    Console.WriteLine($"[PASS] BulkInsertItem 存在");
    var t3 = typeof(ClickHouseIntegration.ConnectionStats);
    Console.WriteLine($"[PASS] ConnectionStats 存在");
    var t4 = typeof(ClickHouseIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t5 = typeof(ClickHouseIntegration.IClickHouseService);
    Console.WriteLine($"[PASS] IClickHouseService 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}