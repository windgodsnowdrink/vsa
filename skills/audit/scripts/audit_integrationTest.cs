#load "audit_integration.cs"

Console.WriteLine("=== audit_integration Test ===");

try
{
    var t0 = typeof(AuditQueue);
    Console.WriteLine($"[PASS] AuditQueue 存在");
    var t1 = typeof(HighPerformanceAuditProvider);
    Console.WriteLine($"[PASS] HighPerformanceAuditProvider 存在");
    var t2 = typeof(AuditBackgroundService);
    Console.WriteLine($"[PASS] AuditBackgroundService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}