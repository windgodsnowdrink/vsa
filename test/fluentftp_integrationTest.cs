#load "fluentftp_integration.cs"

Console.WriteLine("=== fluentftp_integration Test ===");

try
{
    var t0 = typeof(FtpConnectionPool);
    Console.WriteLine($"[PASS] FtpConnectionPool 存在");
    var t1 = typeof(FtpClientPooledObjectPolicy);
    Console.WriteLine($"[PASS] FtpClientPooledObjectPolicy 存在");
    var t2 = typeof(FtpConfig);
    Console.WriteLine($"[PASS] FtpConfig 存在");
    var t3 = typeof(FtpService);
    Console.WriteLine($"[PASS] FtpService 存在");
    var t4 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t5 = typeof(MyService);
    Console.WriteLine($"[PASS] MyService 存在");
    var t6 = typeof(IFtpService);
    Console.WriteLine($"[PASS] IFtpService 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}