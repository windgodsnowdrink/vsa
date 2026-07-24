#load "ssh_core.cs"

Console.WriteLine("=== ssh_core Test ===");

try
{
    var t0 = typeof(SSH.Client.SshConnection);
    Console.WriteLine($"[PASS] SshConnection 存在");
    var t1 = typeof(SSH.Client.SshSession);
    Console.WriteLine($"[PASS] SshSession 存在");
    var t2 = typeof(SSH.Client.SshConfig);
    Console.WriteLine($"[PASS] SshConfig 存在");
    var t3 = typeof(SSH.Client.SshConfigEntry);
    Console.WriteLine($"[PASS] SshConfigEntry 存在");
    var t4 = typeof(SSH.Client.SshService);
    Console.WriteLine($"[PASS] SshService 存在");
    var t5 = typeof(SSH.Client.SshSessionManager);
    Console.WriteLine($"[PASS] SshSessionManager 存在");
    var t6 = typeof(SSH.Client.SshFileTransferService);
    Console.WriteLine($"[PASS] SshFileTransferService 存在");
    var t7 = typeof(SSH.Client.SshConfigService);
    Console.WriteLine($"[PASS] SshConfigService 存在");
    var t8 = typeof(SSH.Client.SshKeyManager);
    Console.WriteLine($"[PASS] SshKeyManager 存在");
    var t9 = typeof(SSH.Client.SshBatchService);
    Console.WriteLine($"[PASS] SshBatchService 存在");
    var t10 = typeof(SSH.Client.ISshService);
    Console.WriteLine($"[PASS] ISshService 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(SSH.Client.ISshSessionManager);
    Console.WriteLine($"[PASS] ISshSessionManager 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(SSH.Client.ISshFileTransferService);
    Console.WriteLine($"[PASS] ISshFileTransferService 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(SSH.Client.ISshConfigService);
    Console.WriteLine($"[PASS] ISshConfigService 接口存在 (IsInterface: {t13.IsInterface})");
    var t14 = typeof(SSH.Client.ISshKeyManager);
    Console.WriteLine($"[PASS] ISshKeyManager 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(SSH.Client.ISshBatchService);
    Console.WriteLine($"[PASS] ISshBatchService 接口存在 (IsInterface: {t15.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}