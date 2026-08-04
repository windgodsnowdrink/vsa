#load "rosnet_integration.cs"

Console.WriteLine("=== rosnet_integration Test ===");

try
{
    var t0 = typeof(RosNetIntegration.RosOptions);
    Console.WriteLine($"[PASS] RosOptions 存在");
    var t1 = typeof(RosNetIntegration.RosService);
    Console.WriteLine($"[PASS] RosService 存在");
    var t2 = typeof(RosNetIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(RosNetIntegration.RosHealthCheck);
    Console.WriteLine($"[PASS] RosHealthCheck 存在");
    var t4 = typeof(RosNetIntegration.RosMetricsExtensions);
    Console.WriteLine($"[PASS] RosMetricsExtensions 存在");
    var t5 = typeof(RosNetIntegration.RosBackgroundService);
    Console.WriteLine($"[PASS] RosBackgroundService 存在");
    var t6 = typeof(RosNetIntegration.IRosService);
    Console.WriteLine($"[PASS] IRosService 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}