#load "hotchocolate_final.cs"

Console.WriteLine("=== hotchocolate_final Test ===");

try
{
    var t0 = typeof(AdaptivePartitionStrategy);
    Console.WriteLine($"[PASS] AdaptivePartitionStrategy 存在");
    var t1 = typeof(HotReloadPolicyUpdater);
    Console.WriteLine($"[PASS] HotReloadPolicyUpdater 存在");
    var t2 = typeof(PartitionOptions);
    Console.WriteLine($"[PASS] PartitionOptions 存在");
    var t3 = typeof(PermissionOptions);
    Console.WriteLine($"[PASS] PermissionOptions 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}