#load "audit_query_endpoint.cs"

Console.WriteLine("=== audit_query_endpoint Test ===");

try
{
    var t0 = typeof(AuditQueryEndpoint);
    Console.WriteLine($"[PASS] AuditQueryEndpoint 存在");
    var t1 = typeof(AuditQueryProcessor);
    Console.WriteLine($"[PASS] AuditQueryProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}