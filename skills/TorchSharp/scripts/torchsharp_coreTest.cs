#load "torchsharp_core.cs"

Console.WriteLine("=== torchsharp_core Test ===");

try
{
    var t0 = typeof(TorchSharp.Skill.ModelInfo);
    Console.WriteLine($"[PASS] ModelInfo 存在");
    var t1 = typeof(TorchSharp.Skill.TorchService);
    Console.WriteLine($"[PASS] TorchService 存在");
    var t2 = typeof(TorchSharp.Skill.TensorService);
    Console.WriteLine($"[PASS] TensorService 存在");
    var t3 = typeof(TorchSharp.Skill.ModelService);
    Console.WriteLine($"[PASS] ModelService 存在");
    var t4 = typeof(TorchSharp.Skill.ImageProcessingService);
    Console.WriteLine($"[PASS] ImageProcessingService 存在");
    var t5 = typeof(TorchSharp.Skill.ScrutorDemoService);
    Console.WriteLine($"[PASS] ScrutorDemoService 存在");
    var t6 = typeof(TorchSharp.Skill.Calculator);
    Console.WriteLine($"[PASS] Calculator 存在");
    var t7 = typeof(TorchSharp.Skill.LoggingCalculator);
    Console.WriteLine($"[PASS] LoggingCalculator 存在");
    var t8 = typeof(TorchSharp.Skill.ITorchService);
    Console.WriteLine($"[PASS] ITorchService 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(TorchSharp.Skill.ITensorService);
    Console.WriteLine($"[PASS] ITensorService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(TorchSharp.Skill.IModelService);
    Console.WriteLine($"[PASS] IModelService 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(TorchSharp.Skill.IImageProcessingService);
    Console.WriteLine($"[PASS] IImageProcessingService 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(TorchSharp.Skill.IScrutorDemoService);
    Console.WriteLine($"[PASS] IScrutorDemoService 接口存在 (IsInterface: {t12.IsInterface})");
    var t13 = typeof(TorchSharp.Skill.ICalculator);
    Console.WriteLine($"[PASS] ICalculator 接口存在 (IsInterface: {t13.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}