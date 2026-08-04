#load "llm_integration.cs"

Console.WriteLine("=== llm_integration Test ===");

try
{
    var t0 = typeof(LlmProcessor);
    Console.WriteLine($"[PASS] LlmProcessor 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(MemoryStreamPooledPolicy);
    Console.WriteLine($"[PASS] MemoryStreamPooledPolicy 存在");
    var t3 = typeof(ILlmPipeline);
    Console.WriteLine($"[PASS] ILlmPipeline 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(LlmOptions);
    Console.WriteLine($"[PASS] LlmOptions record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}