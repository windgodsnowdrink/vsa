#load "openapi_integration.cs"

Console.WriteLine("=== openapi_integration Test ===");

try
{
    var t0 = typeof(OpenApiMetrics);
    Console.WriteLine($"[PASS] OpenApiMetrics 存在");
    var t1 = typeof(CachingSwaggerProvider);
    Console.WriteLine($"[PASS] CachingSwaggerProvider 存在");
    var t2 = typeof(OpenApiMetricsService);
    Console.WriteLine($"[PASS] OpenApiMetricsService 存在");
    var t3 = typeof(OpenApiOptions);
    Console.WriteLine($"[PASS] OpenApiOptions 存在");
    var t4 = typeof(OpenApiExtensions);
    Console.WriteLine($"[PASS] OpenApiExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}