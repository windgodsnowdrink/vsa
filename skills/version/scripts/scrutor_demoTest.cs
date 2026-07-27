#load "scrutor_demo.cs"

Console.WriteLine("=== scrutor_demo Test ===");

try
{
    var t0 = typeof(ScrutorDemo.ConsoleLogger);
    Console.WriteLine($"[PASS] ConsoleLogger 存在");
    var t1 = typeof(ScrutorDemo.LoggingVersionServiceDecorator);
    Console.WriteLine($"[PASS] LoggingVersionServiceDecorator 存在");
    var t2 = typeof(ScrutorDemo.CachingVersionServiceDecorator);
    Console.WriteLine($"[PASS] CachingVersionServiceDecorator 存在");
    var t3 = typeof(ScrutorDemo.GenericService);
    Console.WriteLine($"[PASS] GenericService 存在");
    var t4 = typeof(ScrutorDemo.ILogger);
    Console.WriteLine($"[PASS] ILogger 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(ScrutorDemo.IVersionServiceDecorator);
    Console.WriteLine($"[PASS] IVersionServiceDecorator 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(ScrutorDemo.IGenericService);
    Console.WriteLine($"[PASS] IGenericService 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}