#load "castle_core_integration.cs"

Console.WriteLine("=== castle_core_integration Test ===");

try
{
    var t0 = typeof(MemoryOptimizer);
    Console.WriteLine($"[PASS] MemoryOptimizer 存在");
    var t1 = typeof(HttpProxyInterceptor);
    Console.WriteLine($"[PASS] HttpProxyInterceptor 存在");
    var t2 = typeof(HttpProxyService);
    Console.WriteLine($"[PASS] HttpProxyService 存在");
    var t3 = typeof(IHttpProxyService);
    Console.WriteLine($"[PASS] IHttpProxyService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}