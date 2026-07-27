#load "semantic_kernel_service.cs"

Console.WriteLine("=== semantic_kernel_service Test ===");

try
{
    var t0 = typeof(AiAgentProcessor);
    Console.WriteLine($"[PASS] AiAgentProcessor 存在");
    var t1 = typeof(AgentRequest);
    Console.WriteLine($"[PASS] AgentRequest record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}