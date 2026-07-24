#load "jaeger_integration.cs"

Console.WriteLine("=== jaeger_integration Test ===");

try
{
    var t0 = typeof(JaegerOptions);
    Console.WriteLine($"[PASS] JaegerOptions 存在");
    var t1 = typeof(JaegerExtensions);
    Console.WriteLine($"[PASS] JaegerExtensions 存在");
    var t2 = typeof(JaegerBackgroundService);
    Console.WriteLine($"[PASS] JaegerBackgroundService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}