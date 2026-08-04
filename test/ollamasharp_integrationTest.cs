#load "ollamasharp_integration.cs"

Console.WriteLine("=== ollamasharp_integration Test ===");

try
{
    var t0 = typeof(OllamaOptions);
    Console.WriteLine($"[PASS] OllamaOptions 存在");
    var t1 = typeof(OllamaService);
    Console.WriteLine($"[PASS] OllamaService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(IOllamaService);
    Console.WriteLine($"[PASS] IOllamaService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}