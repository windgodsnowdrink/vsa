#load "prometheus_integration.cs"

Console.WriteLine("=== prometheus_integration Test ===");

try
{
    // 验证 PrometheusExtensions 类
    var extensionsType = typeof(PrometheusExtensions);
    Console.WriteLine($"[PASS] PrometheusExtensions 类型存在: {extensionsType.Name}");

    // 验证 PrometheusAlertRuleManager 类
    var ruleManagerType = typeof(PrometheusAlertRuleManager);
    Console.WriteLine($"[PASS] PrometheusAlertRuleManager 类型存在: {ruleManagerType.Name}");

    // 验证 PrometheusAlertRuleWatcher 类
    var watcherType = typeof(PrometheusAlertRuleWatcher);
    Console.WriteLine($"[PASS] PrometheusAlertRuleWatcher 类型存在: {watcherType.Name}");

    // 验证 GrafanaDashboardSyncService 类
    var syncServiceType = typeof(GrafanaDashboardSyncService);
    Console.WriteLine($"[PASS] GrafanaDashboardSyncService 类型存在: {syncServiceType.Name}");

    // 验证 GrafanaOptions 类
    var optionsType = typeof(GrafanaOptions);
    Console.WriteLine($"[PASS] GrafanaOptions 类型存在: {optionsType.Name}");

    // 验证 PrometheusRemoteStorageWriter 类
    var writerType = typeof(PrometheusRemoteStorageWriter);
    Console.WriteLine($"[PASS] PrometheusRemoteStorageWriter 类型存在: {writerType.Name}");

    // 验证 PrometheusAggregatorService 类
    var aggregatorType = typeof(PrometheusAggregatorService);
    Console.WriteLine($"[PASS] PrometheusAggregatorService 类型存在: {aggregatorType.Name}");

    // 验证 PrometheusBackgroundService 类
    var bgServiceType = typeof(PrometheusBackgroundService);
    Console.WriteLine($"[PASS] PrometheusBackgroundService 类型存在: {bgServiceType.Name}");

    // 验证 PrometheusMiddleware 类
    var middlewareType = typeof(PrometheusMiddleware);
    Console.WriteLine($"[PASS] PrometheusMiddleware 类型存在: {middlewareType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}