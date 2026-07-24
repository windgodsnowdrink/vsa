#load "publicapi_integration.cs"

Console.WriteLine("=== publicapi_integration Test ===");

try
{
    var t0 = typeof(ApiIntegrationOptions);
    Console.WriteLine($"[PASS] ApiIntegrationOptions 存在");
    var t1 = typeof(ApiResponse);
    Console.WriteLine($"[PASS] ApiResponse 存在");
    var t2 = typeof(ApiClientMetrics);
    Console.WriteLine($"[PASS] ApiClientMetrics 存在");
    var t3 = typeof(HealthCheckResult);
    Console.WriteLine($"[PASS] HealthCheckResult 存在");
    var t4 = typeof(ApiClient);
    Console.WriteLine($"[PASS] ApiClient 存在");
    var t5 = typeof(ApiClientExtensions);
    Console.WriteLine($"[PASS] ApiClientExtensions 存在");
    var t6 = typeof(IApiClient);
    Console.WriteLine($"[PASS] IApiClient 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}