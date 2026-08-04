#load "zipkin_integration.cs"

Console.WriteLine("=== zipkin_integration Test ===");

try
{
    var t0 = typeof(ZipkinOptions);
    Console.WriteLine($"[PASS] ZipkinOptions 存在");
    var t1 = typeof(ZipkinService);
    Console.WriteLine($"[PASS] ZipkinService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(ZipkinHealthCheck);
    Console.WriteLine($"[PASS] ZipkinHealthCheck 存在");
    var t4 = typeof(IZipkinService);
    Console.WriteLine($"[PASS] IZipkinService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}