#load "abac_attribute_based_access.cs"

Console.WriteLine("=== abac_attribute_based_access Test ===");

try
{
    var t0 = typeof(AbacRequirement);
    Console.WriteLine($"[PASS] AbacRequirement 存在");
    var t1 = typeof(AbacHandler);
    Console.WriteLine($"[PASS] AbacHandler 存在");
    var t2 = typeof(AbacPolicyProvider);
    Console.WriteLine($"[PASS] AbacPolicyProvider 存在");
    var t3 = typeof(AbacOptions);
    Console.WriteLine($"[PASS] AbacOptions 存在");
    var t4 = typeof(DocumentAbacPolicy);
    Console.WriteLine($"[PASS] DocumentAbacPolicy 存在");
    var t5 = typeof(IAbacPolicy);
    Console.WriteLine($"[PASS] IAbacPolicy 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}