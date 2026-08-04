#load "prompt_integration.cs"

Console.WriteLine("=== prompt_integration Test ===");

try
{
    var t0 = typeof(PromptIntegration.PromptOptions);
    Console.WriteLine($"[PASS] PromptOptions 存在");
    var t1 = typeof(PromptIntegration.PromptService);
    Console.WriteLine($"[PASS] PromptService 存在");
    var t2 = typeof(PromptIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(PromptIntegration.IPromptService);
    Console.WriteLine($"[PASS] IPromptService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}