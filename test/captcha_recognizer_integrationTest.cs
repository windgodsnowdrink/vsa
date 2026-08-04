#load "captcha_recognizer_integration.cs"

Console.WriteLine("=== captcha_recognizer_integration Test ===");

try
{
    var t0 = typeof(CaptchaRecognizerIntegration.CaptchaOptions);
    Console.WriteLine($"[PASS] CaptchaOptions 存在");
    var t1 = typeof(CaptchaRecognizerIntegration.CaptchaHealthStatus);
    Console.WriteLine($"[PASS] CaptchaHealthStatus 存在");
    var t2 = typeof(CaptchaRecognizerIntegration.CaptchaService);
    Console.WriteLine($"[PASS] CaptchaService 存在");
    var t3 = typeof(CaptchaRecognizerIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(CaptchaRecognizerIntegration.CaptchaMetricsService);
    Console.WriteLine($"[PASS] CaptchaMetricsService 存在");
    var t5 = typeof(CaptchaRecognizerIntegration.ICaptchaService);
    Console.WriteLine($"[PASS] ICaptchaService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(CaptchaRecognizerIntegration.CaptchaType);
    Console.WriteLine($"[PASS] CaptchaType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}