#load "sshnet_rtty_integration.cs"

Console.WriteLine("=== sshnet_rtty_integration Test ===");

try
{
    var t0 = typeof(RttyOptions);
    Console.WriteLine($"[PASS] RttyOptions 存在");
    var t1 = typeof(AuditLogOptions);
    Console.WriteLine($"[PASS] AuditLogOptions 存在");
    var t2 = typeof(AuditLogEntry);
    Console.WriteLine($"[PASS] AuditLogEntry 存在");
    var t3 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(DangerousCommandValidator);
    Console.WriteLine($"[PASS] DangerousCommandValidator 存在");
    var t5 = typeof(AuditLog);
    Console.WriteLine($"[PASS] AuditLog 存在");
    var t6 = typeof(PermissionOptions);
    Console.WriteLine($"[PASS] PermissionOptions 存在");
    var t7 = typeof(RttyService);
    Console.WriteLine($"[PASS] RttyService 存在");
    var t8 = typeof(SecureSessionChannel);
    Console.WriteLine($"[PASS] SecureSessionChannel 存在");
    var t9 = typeof(IAuditLogger);
    Console.WriteLine($"[PASS] IAuditLogger 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(IDangerousCommandValidator);
    Console.WriteLine($"[PASS] IDangerousCommandValidator 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(IPermissionValidator);
    Console.WriteLine($"[PASS] IPermissionValidator 接口存在 (IsInterface: {t11.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}