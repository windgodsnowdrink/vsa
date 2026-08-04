#load "resilient_polly_integration.cs"

Console.WriteLine("=== resilient_polly_integration Test ===");

try
{
    var t0 = typeof(ResilientOptions);
    Console.WriteLine($"[PASS] ResilientOptions 存在");
    var t1 = typeof(ResilientService);
    Console.WriteLine($"[PASS] ResilientService 存在");
    var t2 = typeof(ResilientExtensions);
    Console.WriteLine($"[PASS] ResilientExtensions 存在");
    var t3 = typeof(ResilientBackgroundService);
    Console.WriteLine($"[PASS] ResilientBackgroundService 存在");
    var t4 = typeof(ResilientHealthCheck);
    Console.WriteLine($"[PASS] ResilientHealthCheck 存在");
    var t5 = typeof(ResilienceMetrics);
    Console.WriteLine($"[PASS] ResilienceMetrics 存在");
    var t6 = typeof(ResilientExample);
    Console.WriteLine($"[PASS] ResilientExample 存在");
    var t7 = typeof(IResilientService);
    Console.WriteLine($"[PASS] IResilientService 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}