#load "toolgood_words_integration.cs"

Console.WriteLine("=== toolgood_words_integration Test ===");

try
{
    var t0 = typeof(YourNamespace.WordCloudService);
    Console.WriteLine($"[PASS] WordCloudService 存在");
    var t1 = typeof(YourNamespace.WordCloudServiceCollectionExtensions);
    Console.WriteLine($"[PASS] WordCloudServiceCollectionExtensions 存在");
    var t2 = typeof(YourNamespace.WordCloudGeneratorPooledPolicy);
    Console.WriteLine($"[PASS] WordCloudGeneratorPooledPolicy 存在");
    var t3 = typeof(YourNamespace.WordCloudProcessingService);
    Console.WriteLine($"[PASS] WordCloudProcessingService 存在");
    var t4 = typeof(YourNamespace.IWordCloudService);
    Console.WriteLine($"[PASS] IWordCloudService 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(YourNamespace.WordCloudOptions);
    Console.WriteLine($"[PASS] WordCloudOptions record 存在");
    var t6 = typeof(YourNamespace.WordCloudJob);
    Console.WriteLine($"[PASS] WordCloudJob record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}