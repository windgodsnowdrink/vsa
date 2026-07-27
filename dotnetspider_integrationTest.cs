#load "dotnetspider_integration.cs"

Console.WriteLine("=== dotnetspider_integration Test ===");

try
{
    var t0 = typeof(DotnetSpiderIntegration);
    Console.WriteLine($"[PASS] DotnetSpiderIntegration 存在");
    var t1 = typeof(DotnetSpiderService);
    Console.WriteLine($"[PASS] DotnetSpiderService 存在");
    var t2 = typeof(DotnetSpiderOptions);
    Console.WriteLine($"[PASS] DotnetSpiderOptions 存在");
    var t3 = typeof(MySpider);
    Console.WriteLine($"[PASS] MySpider 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}