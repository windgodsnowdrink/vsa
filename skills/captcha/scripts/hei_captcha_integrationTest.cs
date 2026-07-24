#load "hei_captcha_integration.cs"

Console.WriteLine("=== hei_captcha_integration Test ===");

try
{
    var t0 = typeof(CaptchaDemo.CaptchaOptions);
    Console.WriteLine($"[PASS] CaptchaOptions 存在");
    var t1 = typeof(CaptchaDemo.CaptchaMetrics);
    Console.WriteLine($"[PASS] CaptchaMetrics 存在");
    var t2 = typeof(CaptchaDemo.CaptchaHealthStatus);
    Console.WriteLine($"[PASS] CaptchaHealthStatus 存在");
    var t3 = typeof(CaptchaDemo.CaptchaService);
    Console.WriteLine($"[PASS] CaptchaService 存在");
    var t4 = typeof(CaptchaDemo.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t5 = typeof(CaptchaDemo.ICaptchaService);
    Console.WriteLine($"[PASS] ICaptchaService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(CaptchaDemo.CaptchaType);
    Console.WriteLine($"[PASS] CaptchaType enum 存在 (IsEnum: {t6.IsEnum})");
    var t7 = typeof(CaptchaDemo.CaptchaComplexity);
    Console.WriteLine($"[PASS] CaptchaComplexity enum 存在 (IsEnum: {t7.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}