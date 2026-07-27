#load "resilient_polly_integration.cs"

Console.WriteLine("=== resilient_polly_integration Test ===");

try
{
    var t0 = typeof(ResilientOptions);
    Console.WriteLine($"[PASS] ResilientOptions 存在");
    var t1 = typeof(HealthCheckResult);
    Console.WriteLine($"[PASS] HealthCheckResult 存在");
    var t2 = typeof(ResilienceMetrics);
    Console.WriteLine($"[PASS] ResilienceMetrics 存在");
    var t3 = typeof(ResilientService);
    Console.WriteLine($"[PASS] ResilientService 存在");
    var t4 = typeof(ResilientServiceExtensions);
    Console.WriteLine($"[PASS] ResilientServiceExtensions 存在");
    var t5 = typeof(TransientHttpErrorException);
    Console.WriteLine($"[PASS] TransientHttpErrorException 存在");
    var t6 = typeof(WeatherForecast);
    Console.WriteLine($"[PASS] WeatherForecast 存在");
    var t7 = typeof(IResilientStrategy);
    Console.WriteLine($"[PASS] IResilientStrategy 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(IResilientService);
    Console.WriteLine($"[PASS] IResilientService 接口存在 (IsInterface: {t8.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}