#load "microsoft_extensions_ai_integration.cs"

Console.WriteLine("=== microsoft_extensions_ai_integration Test ===");

try
{
    var t0 = typeof(Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIOptions);
    Console.WriteLine($"[PASS] MicrosoftExtensionsAIOptions 存在");
    var t1 = typeof(Microsoft.Extensions.AI.Integration.MicrosoftExtensionsAIService);
    Console.WriteLine($"[PASS] MicrosoftExtensionsAIService 存在");
    var t2 = typeof(Microsoft.Extensions.AI.Integration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(Microsoft.Extensions.AI.Integration.IMicrosoftExtensionsAIService);
    Console.WriteLine($"[PASS] IMicrosoftExtensionsAIService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}