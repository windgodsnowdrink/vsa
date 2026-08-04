#load "choetl_integration.cs"

Console.WriteLine("=== choetl_integration Test ===");

try
{
    var t0 = typeof(ChoETLIntegration.ChoETLOptions);
    Console.WriteLine($"[PASS] ChoETLOptions 存在");
    var t1 = typeof(ChoETLIntegration.DataProcessingResult);
    Console.WriteLine($"[PASS] DataProcessingResult 存在");
    var t2 = typeof(ChoETLIntegration.ETLService);
    Console.WriteLine($"[PASS] ETLService 存在");
    var t3 = typeof(ChoETLIntegration.ChoETLServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ChoETLServiceCollectionExtensions 存在");
    var t4 = typeof(ChoETLIntegration.IETLService);
    Console.WriteLine($"[PASS] IETLService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(ChoETLIntegration.in);
    Console.WriteLine($"[PASS] in record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}