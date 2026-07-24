#load "html_aot.cs"

Console.WriteLine("=== html_aot Test ===");

try
{
    var t0 = typeof(HtmlSettings);
    Console.WriteLine($"[PASS] HtmlSettings 存在");
    var t1 = typeof(HtmlParseResult);
    Console.WriteLine($"[PASS] HtmlParseResult 存在");
    var t2 = typeof(HtmlValidationResult);
    Console.WriteLine($"[PASS] HtmlValidationResult 存在");
    var t3 = typeof(WebsiteScrapeResult);
    Console.WriteLine($"[PASS] WebsiteScrapeResult 存在");
    var t4 = typeof(HtmlService);
    Console.WriteLine($"[PASS] HtmlService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}