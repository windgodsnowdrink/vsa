#load "llvm_ir_optimizer.cs"

Console.WriteLine("=== llvm_ir_optimizer Test ===");

try
{
    var t0 = typeof(LlvmIrOptimizer);
    Console.WriteLine($"[PASS] LlvmIrOptimizer 存在");
    var t1 = typeof(SimdProcessor);
    Console.WriteLine($"[PASS] SimdProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}