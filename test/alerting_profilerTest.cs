#load "alerting_profiler.cs"

Console.WriteLine("=== alerting_profiler Test ===");

try
{
    var t0 = typeof(ProfilerAlertService);
    Console.WriteLine($"[PASS] ProfilerAlertService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}