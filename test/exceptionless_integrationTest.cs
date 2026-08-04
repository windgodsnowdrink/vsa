#load "exceptionless_integration.cs"

Console.WriteLine("=== exceptionless_integration Test ===");

try
{
    var t0 = typeof(ExceptionlessConfig);
    Console.WriteLine($"[PASS] ExceptionlessConfig 存在");
    var t1 = typeof(ExceptionChannel);
    Console.WriteLine($"[PASS] ExceptionChannel 存在");
    var t2 = typeof(ExceptionBackgroundService);
    Console.WriteLine($"[PASS] ExceptionBackgroundService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}