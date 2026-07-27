#load "lazyslidecaptcha_integration.cs"

Console.WriteLine("=== lazyslidecaptcha_integration Test ===");

try
{
    var t0 = typeof(CaptchaOptions);
    Console.WriteLine($"[PASS] CaptchaOptions 存在");
    var t1 = typeof(CaptchaService);
    Console.WriteLine($"[PASS] CaptchaService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(BitmapPooledObjectPolicy);
    Console.WriteLine($"[PASS] BitmapPooledObjectPolicy 存在");
    var t4 = typeof(ICaptchaService);
    Console.WriteLine($"[PASS] ICaptchaService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(CaptchaMetrics);
    Console.WriteLine($"[PASS] CaptchaMetrics record 存在");
    var t6 = typeof(CaptchaMode);
    Console.WriteLine($"[PASS] CaptchaMode enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}