#load "dapr_integration.cs"

Console.WriteLine("=== dapr_integration Test ===");

try
{
    var t0 = typeof(Dapr.Integration.DaprOptions);
    Console.WriteLine($"[PASS] DaprOptions 存在");
    var t1 = typeof(Dapr.Integration.DaprHealthCheckResponse);
    Console.WriteLine($"[PASS] DaprHealthCheckResponse 存在");
    var t2 = typeof(Dapr.Integration.DaprService);
    Console.WriteLine($"[PASS] DaprService 存在");
    var t3 = typeof(Dapr.Integration.DaprServiceExtensions);
    Console.WriteLine($"[PASS] DaprServiceExtensions 存在");
    var t4 = typeof(Dapr.Integration.IDaprService);
    Console.WriteLine($"[PASS] IDaprService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}