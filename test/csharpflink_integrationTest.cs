#load "csharpflink_integration.cs"

Console.WriteLine("=== csharpflink_integration Test ===");

try
{
    var t0 = typeof(FlinkIntegration.FlinkOptions);
    Console.WriteLine($"[PASS] FlinkOptions 存在");
    var t1 = typeof(FlinkIntegration.FlinkService);
    Console.WriteLine($"[PASS] FlinkService 存在");
    var t2 = typeof(FlinkIntegration.FlinkServiceExtensions);
    Console.WriteLine($"[PASS] FlinkServiceExtensions 存在");
    var t3 = typeof(FlinkIntegration.IFlinkService);
    Console.WriteLine($"[PASS] IFlinkService 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(FlinkIntegration.FlinkResult);
    Console.WriteLine($"[PASS] FlinkResult record 存在");
    var t5 = typeof(FlinkIntegration.FlinkMetrics);
    Console.WriteLine($"[PASS] FlinkMetrics record 存在");
    var t6 = typeof(FlinkIntegration.FlinkJob);
    Console.WriteLine($"[PASS] FlinkJob record 存在");
    var t7 = typeof(FlinkIntegration.WindowResult);
    Console.WriteLine($"[PASS] WindowResult record 存在");
    var t8 = typeof(FlinkIntegration.StateSnapshot);
    Console.WriteLine($"[PASS] StateSnapshot record 存在");
    var t9 = typeof(FlinkIntegration.Watermark);
    Console.WriteLine($"[PASS] Watermark record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}