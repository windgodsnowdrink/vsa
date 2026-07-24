#load "distributed_audit_enhanced.cs"

Console.WriteLine("=== distributed_audit_enhanced Test ===");

try
{
    var t0 = typeof(AuditEventPublisher);
    Console.WriteLine($"[PASS] AuditEventPublisher 存在");
    var t1 = typeof(AuditEventConsumer);
    Console.WriteLine($"[PASS] AuditEventConsumer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}