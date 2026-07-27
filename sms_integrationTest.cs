#load "sms_integration.cs"

Console.WriteLine("=== sms_integration Test ===");

try
{
    var t0 = typeof(SmsOptions);
    Console.WriteLine($"[PASS] SmsOptions 存在");
    var t1 = typeof(SmsSender);
    Console.WriteLine($"[PASS] SmsSender 存在");
    var t2 = typeof(SmsExtensions);
    Console.WriteLine($"[PASS] SmsExtensions 存在");
    var t3 = typeof(ISmsSender);
    Console.WriteLine($"[PASS] ISmsSender 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}