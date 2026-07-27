#load "sms_core.cs"

Console.WriteLine("=== sms_core Test ===");

try
{
    var t0 = typeof(SmsSkill.SmsResult);
    Console.WriteLine($"[PASS] SmsResult 存在");
    var t1 = typeof(SmsSkill.SmsTemplate);
    Console.WriteLine($"[PASS] SmsTemplate 存在");
    var t2 = typeof(SmsSkill.SmsScheduleResult);
    Console.WriteLine($"[PASS] SmsScheduleResult 存在");
    var t3 = typeof(SmsSkill.SmsAnalytics);
    Console.WriteLine($"[PASS] SmsAnalytics 存在");
    var t4 = typeof(SmsSkill.SmsEvent);
    Console.WriteLine($"[PASS] SmsEvent 存在");
    var t5 = typeof(SmsSkill.SmsMessage);
    Console.WriteLine($"[PASS] SmsMessage 存在");
    var t6 = typeof(SmsSkill.TwilioOptions);
    Console.WriteLine($"[PASS] TwilioOptions 存在");
    var t7 = typeof(SmsSkill.SmsOptions);
    Console.WriteLine($"[PASS] SmsOptions 存在");
    var t8 = typeof(SmsSkill.TwilioSmsProvider);
    Console.WriteLine($"[PASS] TwilioSmsProvider 存在");
    var t9 = typeof(SmsSkill.TemplateService);
    Console.WriteLine($"[PASS] TemplateService 存在");
    var t10 = typeof(SmsSkill.AnalyticsService);
    Console.WriteLine($"[PASS] AnalyticsService 存在");
    var t11 = typeof(SmsSkill.SmsService);
    Console.WriteLine($"[PASS] SmsService 存在");
    var t12 = typeof(SmsSkill.SmsServiceCollectionExtensions);
    Console.WriteLine($"[PASS] SmsServiceCollectionExtensions 存在");
    var t13 = typeof(SmsSkill.ISmsProvider);
    Console.WriteLine($"[PASS] ISmsProvider 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(SmsSkill.ITemplateService);
    Console.WriteLine($"[PASS] ITemplateService 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(SmsSkill.IAnalyticsService);
    Console.WriteLine($"[PASS] IAnalyticsService 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(SmsSkill.ISmsService);
    Console.WriteLine($"[PASS] ISmsService 接口存在 (IsInterface: {t16.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}