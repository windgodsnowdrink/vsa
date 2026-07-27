#load "handycontrol_integration.cs"

Console.WriteLine("=== handycontrol_integration Test ===");

try
{
    var t0 = typeof(HandyControlOptions);
    Console.WriteLine($"[PASS] HandyControlOptions 存在");
    var t1 = typeof(HandyControlService);
    Console.WriteLine($"[PASS] HandyControlService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(IHandyControlService);
    Console.WriteLine($"[PASS] IHandyControlService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}