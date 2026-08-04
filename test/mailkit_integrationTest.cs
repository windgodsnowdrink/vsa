#load "mailkit_integration.cs"

Console.WriteLine("=== mailkit_integration Test ===");

try
{
    var t0 = typeof(SmtpOptions);
    Console.WriteLine($"[PASS] SmtpOptions 存在");
    var t1 = typeof(EmailService);
    Console.WriteLine($"[PASS] EmailService 存在");
    var t2 = typeof(SmtpClientPooledPolicy);
    Console.WriteLine($"[PASS] SmtpClientPooledPolicy 存在");
    var t3 = typeof(EmailMessage);
    Console.WriteLine($"[PASS] EmailMessage 存在");
    var t4 = typeof(EmailAttachment);
    Console.WriteLine($"[PASS] EmailAttachment 存在");
    var t5 = typeof(EmailServiceExtensions);
    Console.WriteLine($"[PASS] EmailServiceExtensions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}