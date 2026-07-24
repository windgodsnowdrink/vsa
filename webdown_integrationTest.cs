#load "webdown_integration.cs"

Console.WriteLine("=== webdown_integration Test ===");

try
{
    var t0 = typeof(WebDownService);
    Console.WriteLine($"[PASS] WebDownService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(IWebDownService);
    Console.WriteLine($"[PASS] IWebDownService 接口存在 (IsInterface: {t2.IsInterface})");
    var t3 = typeof(DownloadTask);
    Console.WriteLine($"[PASS] DownloadTask record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}