#load "ulid_service.cs"

Console.WriteLine("=== ulid_service Test ===");

try
{
    var t0 = typeof(UlidGenerator);
    Console.WriteLine($"[PASS] UlidGenerator 存在");
    var t1 = typeof(UlidRequest);
    Console.WriteLine($"[PASS] UlidRequest 存在");
    var t2 = typeof(UlidContext);
    Console.WriteLine($"[PASS] UlidContext 存在");
    var t3 = typeof(UlidContextPooledPolicy);
    Console.WriteLine($"[PASS] UlidContextPooledPolicy 存在");
    var t4 = typeof(Order);
    Console.WriteLine($"[PASS] Order 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}