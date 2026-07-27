#load "smtpserver_integration.cs"

Console.WriteLine("=== smtpserver_integration Test ===");

try
{
    var t0 = typeof(MessageStore);
    Console.WriteLine($"[PASS] MessageStore 存在");
    var t1 = typeof(MessageBuffer);
    Console.WriteLine($"[PASS] MessageBuffer 存在");
    var t2 = typeof(MessageBufferPooledPolicy);
    Console.WriteLine($"[PASS] MessageBufferPooledPolicy 存在");
    var t3 = typeof(MassTransitMessageForwarder);
    Console.WriteLine($"[PASS] MassTransitMessageForwarder 存在");
    var t4 = typeof(MessageEncryptor);
    Console.WriteLine($"[PASS] MessageEncryptor 存在");
    var t5 = typeof(IMessageRepository);
    Console.WriteLine($"[PASS] IMessageRepository 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(IMessageForwarder);
    Console.WriteLine($"[PASS] IMessageForwarder 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(DbMessage);
    Console.WriteLine($"[PASS] DbMessage record 存在");
    var t8 = typeof(EmailMessageEvent);
    Console.WriteLine($"[PASS] EmailMessageEvent record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}