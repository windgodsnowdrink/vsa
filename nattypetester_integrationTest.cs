#load "nattypetester_integration.cs"

Console.WriteLine("=== nattypetester_integration Test ===");

try
{
    var t0 = typeof(NatTypeTesterOptions);
    Console.WriteLine($"[PASS] NatTypeTesterOptions 存在");
    var t1 = typeof(NatTypeTester);
    Console.WriteLine($"[PASS] NatTypeTester 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(UdpClientPooledObjectPolicy);
    Console.WriteLine($"[PASS] UdpClientPooledObjectPolicy 存在");
    var t4 = typeof(INatTypeTester);
    Console.WriteLine($"[PASS] INatTypeTester 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}