#load "chaos_integration.cs"

Console.WriteLine("=== chaos_integration Test ===");

try
{
    var t0 = typeof(ChaosOptions);
    Console.WriteLine($"[PASS] ChaosOptions 存在");
    var t1 = typeof(ChaosService);
    Console.WriteLine($"[PASS] ChaosService 存在");
    var t2 = typeof(ChaosExtensions);
    Console.WriteLine($"[PASS] ChaosExtensions 存在");
    var t3 = typeof(ChaosBackgroundService);
    Console.WriteLine($"[PASS] ChaosBackgroundService 存在");
    var t4 = typeof(IChaosService);
    Console.WriteLine($"[PASS] IChaosService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}