#load "prometheus_integration.cs"

Console.WriteLine("=== prometheus_integration Test ===");

try
{
    var t0 = typeof(PrometheusExtensions);
    Console.WriteLine($"[PASS] PrometheusExtensions 存在");
    var t1 = typeof(PrometheusAlertRuleManager);
    Console.WriteLine($"[PASS] PrometheusAlertRuleManager 存在");
    var t2 = typeof(PrometheusAlertRuleWatcher);
    Console.WriteLine($"[PASS] PrometheusAlertRuleWatcher 存在");
    var t3 = typeof(GrafanaDashboardSyncService);
    Console.WriteLine($"[PASS] GrafanaDashboardSyncService 存在");
    var t4 = typeof(GrafanaOptions);
    Console.WriteLine($"[PASS] GrafanaOptions 存在");
    var t5 = typeof(PrometheusRemoteStorageWriter);
    Console.WriteLine($"[PASS] PrometheusRemoteStorageWriter 存在");
    var t6 = typeof(PrometheusAggregatorService);
    Console.WriteLine($"[PASS] PrometheusAggregatorService 存在");
    var t7 = typeof(PrometheusBackgroundService);
    Console.WriteLine($"[PASS] PrometheusBackgroundService 存在");
    var t8 = typeof(PrometheusMiddleware);
    Console.WriteLine($"[PASS] PrometheusMiddleware 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}