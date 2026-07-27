#load "hotchocolate_production.cs"

Console.WriteLine("=== hotchocolate_production Test ===");

try
{
    var t0 = typeof(HashBasedPartitionStrategy);
    Console.WriteLine($"[PASS] HashBasedPartitionStrategy 存在");
    var t1 = typeof(PermissionDirectiveType);
    Console.WriteLine($"[PASS] PermissionDirectiveType 存在");
    var t2 = typeof(PermissionDirective);
    Console.WriteLine($"[PASS] PermissionDirective 存在");
    var t3 = typeof(MonitoredDataLoader);
    Console.WriteLine($"[PASS] MonitoredDataLoader 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}