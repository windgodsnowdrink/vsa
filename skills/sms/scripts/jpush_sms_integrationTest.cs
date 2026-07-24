#load "jpush_sms_integration.cs"

Console.WriteLine("=== jpush_sms_integration Test ===");

try
{
    var t0 = typeof(RateLimiter);
    Console.WriteLine($"[PASS] RateLimiter 存在");
    var t1 = typeof(JpushSmsOptions);
    Console.WriteLine($"[PASS] JpushSmsOptions 存在");
    var t2 = typeof(JpushSmsService);
    Console.WriteLine($"[PASS] JpushSmsService 存在");
    var t3 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(IJpushSmsService);
    Console.WriteLine($"[PASS] IJpushSmsService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(SmsMessage);
    Console.WriteLine($"[PASS] SmsMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}