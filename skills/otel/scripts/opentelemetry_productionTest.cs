#load "opentelemetry_production.cs"

Console.WriteLine("=== opentelemetry_production Test ===");

try
{
    var t0 = typeof(CustomSpanExporter);
    Console.WriteLine($"[PASS] CustomSpanExporter 存在");
    var t1 = typeof(DynamicSampler);
    Console.WriteLine($"[PASS] DynamicSampler 存在");
    var t2 = typeof(ActivityPool);
    Console.WriteLine($"[PASS] ActivityPool 存在");
    var t3 = typeof(ResourceDiscoveryService);
    Console.WriteLine($"[PASS] ResourceDiscoveryService 存在");
    var t4 = typeof(AlertingService);
    Console.WriteLine($"[PASS] AlertingService 存在");
    var t5 = typeof(Telemetry);
    Console.WriteLine($"[PASS] Telemetry 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}