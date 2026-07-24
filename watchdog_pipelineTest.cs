#load "watchdog_pipeline.cs"

Console.WriteLine("=== watchdog_pipeline Test ===");

try
{
    var t0 = typeof(LogPipeline);
    Console.WriteLine($"[PASS] LogPipeline 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}