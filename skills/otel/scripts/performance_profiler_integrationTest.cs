#load "performance_profiler_integration.cs"

Console.WriteLine("=== performance_profiler_integration Test ===");

try
{
    var t0 = typeof(PerformanceProfilerOptions);
    Console.WriteLine($"[PASS] PerformanceProfilerOptions 存在");
    var t1 = typeof(DotTraceProfiler);
    Console.WriteLine($"[PASS] DotTraceProfiler 存在");
    var t2 = typeof(PerformanceProfilerExtensions);
    Console.WriteLine($"[PASS] PerformanceProfilerExtensions 存在");
    var t3 = typeof(IPerformanceProfiler);
    Console.WriteLine($"[PASS] IPerformanceProfiler 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}