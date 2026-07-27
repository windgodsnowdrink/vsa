#load "watchdog_pipeline.cs"

Console.WriteLine("=== watchdog_pipeline Test ===");

try
{
    // 验证 LogPipeline 类
    var pipelineType = typeof(LogPipeline);
    Console.WriteLine($"[PASS] LogPipeline 类型存在: {pipelineType.Name}");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}