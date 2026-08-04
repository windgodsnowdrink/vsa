#load "scalar_integration.cs"

Console.WriteLine("=== scalar_integration Test ===");

try
{
    var t0 = typeof(ScalarOptions);
    Console.WriteLine($"[PASS] ScalarOptions 存在");
    var t1 = typeof(ScalarService);
    Console.WriteLine($"[PASS] ScalarService 存在");
    var t2 = typeof(ScalarExtensions);
    Console.WriteLine($"[PASS] ScalarExtensions 存在");
    var t3 = typeof(ScalarBackgroundService);
    Console.WriteLine($"[PASS] ScalarBackgroundService 存在");
    var t4 = typeof(ScalarController);
    Console.WriteLine($"[PASS] ScalarController 存在");
    var t5 = typeof(IScalarService);
    Console.WriteLine($"[PASS] IScalarService 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}