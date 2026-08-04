#load "performance_interceptor_integration.cs"

Console.WriteLine("=== performance_interceptor_integration Test ===");

try
{
    var t0 = typeof(PerformanceInterceptor);
    Console.WriteLine($"[PASS] PerformanceInterceptor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}