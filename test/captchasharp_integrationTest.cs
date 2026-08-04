#load "captchasharp_integration.cs"

Console.WriteLine("=== captchasharp_integration Test ===");

try
{
    var t0 = typeof(CaptchaSharpIntegration.CaptchaOptions);
    Console.WriteLine($"[PASS] CaptchaOptions 存在");
    var t1 = typeof(CaptchaSharpIntegration.CaptchaMetrics);
    Console.WriteLine($"[PASS] CaptchaMetrics 存在");
    var t2 = typeof(CaptchaSharpIntegration.CaptchaHealthStatus);
    Console.WriteLine($"[PASS] CaptchaHealthStatus 存在");
    var t3 = typeof(CaptchaSharpIntegration.CaptchaService);
    Console.WriteLine($"[PASS] CaptchaService 存在");
    var t4 = typeof(CaptchaSharpIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t5 = typeof(CaptchaSharpIntegration.CaptchaMetricsService);
    Console.WriteLine($"[PASS] CaptchaMetricsService 存在");
    var t6 = typeof(CaptchaSharpIntegration.ICaptchaService);
    Console.WriteLine($"[PASS] ICaptchaService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(CaptchaSharpIntegration.CaptchaType);
    Console.WriteLine($"[PASS] CaptchaType enum 存在 (IsEnum: {t7.IsEnum})");
    var t8 = typeof(CaptchaSharpIntegration.CaptchaComplexity);
    Console.WriteLine($"[PASS] CaptchaComplexity enum 存在 (IsEnum: {t8.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}