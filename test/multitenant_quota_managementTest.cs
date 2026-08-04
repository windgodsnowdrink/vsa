#load "multitenant_quota_management.cs"

Console.WriteLine("=== multitenant_quota_management Test ===");

try
{
    var t0 = typeof(TenantQuotaService);
    Console.WriteLine($"[PASS] TenantQuotaService 存在");
    var t1 = typeof(QuotaContext);
    Console.WriteLine($"[PASS] QuotaContext 存在");
    var t2 = typeof(TenantQuotaExtensions);
    Console.WriteLine($"[PASS] TenantQuotaExtensions 存在");
    var t3 = typeof(TenantQuota);
    Console.WriteLine($"[PASS] TenantQuota struct 存在");
    var t4 = typeof(TenantResourceType);
    Console.WriteLine($"[PASS] TenantResourceType enum 存在 (IsEnum: {t4.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}