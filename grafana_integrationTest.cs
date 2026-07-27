#load "grafana_integration.cs"

Console.WriteLine("=== grafana_integration Test ===");

try
{
    var t0 = typeof(GrafanaExtensions);
    Console.WriteLine($"[PASS] GrafanaExtensions 存在");
    var t1 = typeof(GrafanaOptions);
    Console.WriteLine($"[PASS] GrafanaOptions 存在");
    var t2 = typeof(GrafanaClient);
    Console.WriteLine($"[PASS] GrafanaClient 存在");
    var t3 = typeof(GrafanaDashboardManager);
    Console.WriteLine($"[PASS] GrafanaDashboardManager 存在");
    var t4 = typeof(GrafanaInitializationService);
    Console.WriteLine($"[PASS] GrafanaInitializationService 存在");
    var t5 = typeof(PollyExtensions);
    Console.WriteLine($"[PASS] PollyExtensions 存在");
    var t6 = typeof(IGrafanaDashboardManager);
    Console.WriteLine($"[PASS] IGrafanaDashboardManager 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}