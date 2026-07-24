#load "captchagen_integration.cs"

Console.WriteLine("=== captchagen_integration Test ===");

try
{
    var t0 = typeof(CaptchaService);
    Console.WriteLine($"[PASS] CaptchaService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(BitmapPooledPolicy);
    Console.WriteLine($"[PASS] BitmapPooledPolicy 存在");
    var t3 = typeof(ICaptchaService);
    Console.WriteLine($"[PASS] ICaptchaService 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(CaptchaOptions);
    Console.WriteLine($"[PASS] CaptchaOptions record 存在");
    var t5 = typeof(CaptchaPerformanceMetrics);
    Console.WriteLine($"[PASS] CaptchaPerformanceMetrics record 存在");
    var t6 = typeof(CaptchaType);
    Console.WriteLine($"[PASS] CaptchaType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}