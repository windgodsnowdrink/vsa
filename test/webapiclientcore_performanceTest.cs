#load "webapiclientcore_performance.cs"

Console.WriteLine("=== webapiclientcore_performance Test ===");

try
{
    var t0 = typeof(LlvmIrOptimizer);
    Console.WriteLine($"[PASS] LlvmIrOptimizer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}