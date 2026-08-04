#load "saga_orchestrator.cs"

Console.WriteLine("=== saga_orchestrator Test ===");

try
{
    var t0 = typeof(SagaOrchestrator);
    Console.WriteLine($"[PASS] SagaOrchestrator 存在");
    var t1 = typeof(SagaCommand);
    Console.WriteLine($"[PASS] SagaCommand record 存在");
    var t2 = typeof(SagaStep);
    Console.WriteLine($"[PASS] SagaStep record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}