#load "papercut_integration.cs"

Console.WriteLine("=== papercut_integration Test ===");

try
{
    var t0 = typeof(EmailProcessingService);
    Console.WriteLine($"[PASS] EmailProcessingService 存在");
    var t1 = typeof(MimePartPool);
    Console.WriteLine($"[PASS] MimePartPool 存在");
    var t2 = typeof(MimePartPooledPolicy);
    Console.WriteLine($"[PASS] MimePartPooledPolicy 存在");
    var t3 = typeof(EmailServerExtensions);
    Console.WriteLine($"[PASS] EmailServerExtensions 存在");
    var t4 = typeof(EmailMessage);
    Console.WriteLine($"[PASS] EmailMessage record 存在");
    var t5 = typeof(LargeMessage);
    Console.WriteLine($"[PASS] LargeMessage struct 存在");
    var t6 = typeof(DynamicMessage);
    Console.WriteLine($"[PASS] DynamicMessage struct 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}