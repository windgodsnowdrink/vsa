#load "botsharp_integration.cs"

Console.WriteLine("=== botsharp_integration Test ===");

try
{
    var t0 = typeof(BotSharpServiceCollectionExtensions);
    Console.WriteLine($"[PASS] BotSharpServiceCollectionExtensions 存在");
    var t1 = typeof(BotSharpProcessor);
    Console.WriteLine($"[PASS] BotSharpProcessor 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(IBotSharpPipeline);
    Console.WriteLine($"[PASS] IBotSharpPipeline 接口存在 (IsInterface: {t3.IsInterface})");
    var t4 = typeof(BotSharpOptions);
    Console.WriteLine($"[PASS] BotSharpOptions record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}