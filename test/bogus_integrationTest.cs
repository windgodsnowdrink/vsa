#load "bogus_integration.cs"

Console.WriteLine("=== bogus_integration Test ===");

try
{
    var t0 = typeof(BogusServiceExtensions);
    Console.WriteLine($"[PASS] BogusServiceExtensions 存在");
    var t1 = typeof(BogusDataGenerator);
    Console.WriteLine($"[PASS] BogusDataGenerator 存在");
    var t2 = typeof(IDataGenerator);
    Console.WriteLine($"[PASS] IDataGenerator 接口存在 (IsInterface: {t2.IsInterface})");
    var t3 = typeof(User);
    Console.WriteLine($"[PASS] User record 存在");
    var t4 = typeof(Product);
    Console.WriteLine($"[PASS] Product record 存在");
    var t5 = typeof(Order);
    Console.WriteLine($"[PASS] Order record 存在");
    var t6 = typeof(OrderItem);
    Console.WriteLine($"[PASS] OrderItem record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}