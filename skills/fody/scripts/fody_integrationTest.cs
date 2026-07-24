#load "fody_integration.cs"

Console.WriteLine("=== fody_integration Test ===");

try
{
    var t0 = typeof(LogAttribute);
    Console.WriteLine($"[PASS] LogAttribute 存在");
    var t1 = typeof(LogContext);
    Console.WriteLine($"[PASS] LogContext 存在");
    var t2 = typeof(LogProcessor);
    Console.WriteLine($"[PASS] LogProcessor 存在");
    var t3 = typeof(MethodTimerAttribute);
    Console.WriteLine($"[PASS] MethodTimerAttribute 存在");
    var t4 = typeof(AotFriendlyProxy);
    Console.WriteLine($"[PASS] AotFriendlyProxy 存在");
    var t5 = typeof(DynamicCompilationInterceptor);
    Console.WriteLine($"[PASS] DynamicCompilationInterceptor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}