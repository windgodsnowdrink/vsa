#load "nodatime_extensions.cs"

Console.WriteLine("=== nodatime_extensions Test ===");

try
{
    var t0 = typeof(NodaTimeOptions);
    Console.WriteLine($"[PASS] NodaTimeOptions 存在");
    var t1 = typeof(TimeRequest);
    Console.WriteLine($"[PASS] TimeRequest 存在");
    var t2 = typeof(NodaTimeContext);
    Console.WriteLine($"[PASS] NodaTimeContext 存在");
    var t3 = typeof(NodaTimeContextPooledPolicy);
    Console.WriteLine($"[PASS] NodaTimeContextPooledPolicy 存在");
    var t4 = typeof(TailLatencyOptimizer);
    Console.WriteLine($"[PASS] TailLatencyOptimizer 存在");
    var t5 = typeof(NodaTimeService);
    Console.WriteLine($"[PASS] NodaTimeService 存在");
    var t6 = typeof(NodaTimeCalculator);
    Console.WriteLine($"[PASS] NodaTimeCalculator 存在");
    var t7 = typeof(DateTimeZoneCache);
    Console.WriteLine($"[PASS] DateTimeZoneCache 存在");
    var t8 = typeof(NodaTimeServiceCollectionExtensions);
    Console.WriteLine($"[PASS] NodaTimeServiceCollectionExtensions 存在");
    var t9 = typeof(INodaTimeService);
    Console.WriteLine($"[PASS] INodaTimeService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(INodaTimeCalculator);
    Console.WriteLine($"[PASS] INodaTimeCalculator 接口存在 (IsInterface: {t10.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}