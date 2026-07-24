#load "ml_service_pipeline.cs"

Console.WriteLine("=== ml_service_pipeline Test ===");

try
{
    var t0 = typeof(MlModelPipeline);
    Console.WriteLine($"[PASS] MlModelPipeline 存在");
    var t1 = typeof(ModelInput);
    Console.WriteLine($"[PASS] ModelInput 存在");
    var t2 = typeof(ModelOutput);
    Console.WriteLine($"[PASS] ModelOutput 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}