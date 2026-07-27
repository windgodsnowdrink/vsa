#load "distributed_audit_store.cs"

Console.WriteLine("=== distributed_audit_store Test ===");

try
{
    var t0 = typeof(DistributedAuditWriter);
    Console.WriteLine($"[PASS] DistributedAuditWriter 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}