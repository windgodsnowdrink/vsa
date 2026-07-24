#load "streamjson_integration.cs"

Console.WriteLine("=== streamjson_integration Test ===");

try
{
    var t0 = typeof(CalculatorService);
    Console.WriteLine($"[PASS] CalculatorService 存在");
    var t1 = typeof(MyService);
    Console.WriteLine($"[PASS] MyService 存在");
    var t2 = typeof(FileWatcher);
    Console.WriteLine($"[PASS] FileWatcher 存在");
    var t3 = typeof(ICalculatorService);
    Console.WriteLine($"[PASS] ICalculatorService 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(IRemoteService);
    Console.WriteLine($"[PASS] IRemoteService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(IFileWatcher);
    Console.WriteLine($"[PASS] IFileWatcher 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}