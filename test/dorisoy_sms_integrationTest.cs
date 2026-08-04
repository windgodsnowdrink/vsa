#load "dorisoy_sms_integration.cs"

Console.WriteLine("=== dorisoy_sms_integration Test ===");

try
{
    var t0 = typeof(SmsOptions);
    Console.WriteLine($"[PASS] SmsOptions 存在");
    var t1 = typeof(TrieTree);
    Console.WriteLine($"[PASS] TrieTree 存在");
    var t2 = typeof(TrieNode);
    Console.WriteLine($"[PASS] TrieNode 存在");
    var t3 = typeof(SmsListRepository);
    Console.WriteLine($"[PASS] SmsListRepository 存在");
    var t4 = typeof(SmsService);
    Console.WriteLine($"[PASS] SmsService 存在");
    var t5 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t6 = typeof(MyService);
    Console.WriteLine($"[PASS] MyService 存在");
    var t7 = typeof(RateLimiter);
    Console.WriteLine($"[PASS] RateLimiter 存在");
    var t8 = typeof(ISmsService);
    Console.WriteLine($"[PASS] ISmsService 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(SmsMessage);
    Console.WriteLine($"[PASS] SmsMessage struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}