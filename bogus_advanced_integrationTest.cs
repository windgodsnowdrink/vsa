#load "bogus_advanced_integration.cs"

Console.WriteLine("=== bogus_advanced_integration Test ===");

try
{
    var t0 = typeof(OrderContext);
    Console.WriteLine($"[PASS] OrderContext 存在");
    var t1 = typeof(MultiLingualDataGenerator);
    Console.WriteLine($"[PASS] MultiLingualDataGenerator 存在");
    var t2 = typeof(CustomBogusRules);
    Console.WriteLine($"[PASS] CustomBogusRules 存在");
    var t3 = typeof(BogusServiceCollectionExtensions);
    Console.WriteLine($"[PASS] BogusServiceCollectionExtensions 存在");
    var t4 = typeof(BogusAdvancedDemo);
    Console.WriteLine($"[PASS] BogusAdvancedDemo 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}