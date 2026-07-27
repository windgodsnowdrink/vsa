#load "multitenant_lifecycle.cs"

Console.WriteLine("=== multitenant_lifecycle Test ===");

try
{
    var t0 = typeof(TenantLifecycleProcessor);
    Console.WriteLine($"[PASS] TenantLifecycleProcessor 存在");
    var t1 = typeof(TenantEventContext);
    Console.WriteLine($"[PASS] TenantEventContext 存在");
    var t2 = typeof(TenantEventContextPooledPolicy);
    Console.WriteLine($"[PASS] TenantEventContextPooledPolicy 存在");
    var t3 = typeof(TenantLifecycleExtensions);
    Console.WriteLine($"[PASS] TenantLifecycleExtensions 存在");
    var t4 = typeof(TenantLifecycleEvent);
    Console.WriteLine($"[PASS] TenantLifecycleEvent struct 存在");
    var t5 = typeof(TenantLifecycleEventType);
    Console.WriteLine($"[PASS] TenantLifecycleEventType enum 存在 (IsEnum: {t5.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}