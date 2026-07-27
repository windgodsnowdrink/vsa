#load "multistage_pipeline.cs"

Console.WriteLine("=== multistage_pipeline Test ===");

try
{
    var t0 = typeof(MultiStagePipeline);
    Console.WriteLine($"[PASS] MultiStagePipeline 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}