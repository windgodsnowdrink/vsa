#load "mysql_to_sqlite_litedb_sync.cs"

Console.WriteLine("=== mysql_to_sqlite_litedb_sync Test ===");

try
{
    var t0 = typeof(DataSyncService);
    Console.WriteLine($"[PASS] DataSyncService 存在");
    var t1 = typeof(LiteDbSyncItem);
    Console.WriteLine($"[PASS] LiteDbSyncItem 存在");
    var t2 = typeof(DataSyncController);
    Console.WriteLine($"[PASS] DataSyncController 存在");
    var t3 = typeof(MetricFactoryExtensions);
    Console.WriteLine($"[PASS] MetricFactoryExtensions 存在");
    var t4 = typeof(MockGaugeMetricFamily);
    Console.WriteLine($"[PASS] MockGaugeMetricFamily 存在");
    var t5 = typeof(MockGauge);
    Console.WriteLine($"[PASS] MockGauge 存在");
    var t6 = typeof(Collector);
    Console.WriteLine($"[PASS] Collector 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(IMetricFamily);
    Console.WriteLine($"[PASS] IMetricFamily 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(IGauge);
    Console.WriteLine($"[PASS] IGauge 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(IMetricFactory);
    Console.WriteLine($"[PASS] IMetricFactory 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(SyncData);
    Console.WriteLine($"[PASS] SyncData record 存在");
    var t11 = typeof(SyncStatus);
    Console.WriteLine($"[PASS] SyncStatus enum 存在 (IsEnum: {t11.IsEnum})");
    var t12 = typeof(MetricType);
    Console.WriteLine($"[PASS] MetricType enum 存在 (IsEnum: {t12.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}