#load "tensorflowsharp_integration.cs"

Console.WriteLine("=== tensorflowsharp_integration Test ===");

try
{
    var t0 = typeof(Trae.Vsa.AI.TensorFlowOptions);
    Console.WriteLine($"[PASS] TensorFlowOptions 存在");
    var t1 = typeof(Trae.Vsa.AI.TensorFlowService);
    Console.WriteLine($"[PASS] TensorFlowService 存在");
    var t2 = typeof(Trae.Vsa.AI.TensorFlowExtensions);
    Console.WriteLine($"[PASS] TensorFlowExtensions 存在");
    var t3 = typeof(Trae.Vsa.AI.TensorFlowModelVersioningService);
    Console.WriteLine($"[PASS] TensorFlowModelVersioningService 存在");
    var t4 = typeof(Trae.Vsa.AI.ITensorFlowService);
    Console.WriteLine($"[PASS] ITensorFlowService 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}