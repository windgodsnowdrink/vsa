#load "dataflow_pipeline.cs"

Console.WriteLine("=== dataflow_pipeline Test ===");

try
{
    var t0 = typeof(DataflowMessagePipeline);
    Console.WriteLine($"[PASS] DataflowMessagePipeline 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}