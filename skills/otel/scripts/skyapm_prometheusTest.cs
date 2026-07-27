#load "skyapm_prometheus.cs"

Console.WriteLine("=== skyapm_prometheus Test ===");

try
{
    // 验证 PrometheusExporter 类
    var exporterType = typeof(PrometheusExporter);
    Console.WriteLine($"[PASS] PrometheusExporter 类型存在: {exporterType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}