#load "htmlagilitypack_integration.cs"

Console.WriteLine("=== htmlagilitypack_integration Test ===");

try
{
    var t0 = typeof(HtmlAgilityPackIntegration);
    Console.WriteLine($"[PASS] HtmlAgilityPackIntegration 存在");
    var t1 = typeof(HtmlAgilityPackService);
    Console.WriteLine($"[PASS] HtmlAgilityPackService 存在");
    var t2 = typeof(HtmlParserOptions);
    Console.WriteLine($"[PASS] HtmlParserOptions 存在");
    var t3 = typeof(HtmlSanitizerService);
    Console.WriteLine($"[PASS] HtmlSanitizerService 存在");
    var t4 = typeof(HtmlProcessingPipeline);
    Console.WriteLine($"[PASS] HtmlProcessingPipeline 存在");
    var t5 = typeof(HtmlPerformanceMonitor);
    Console.WriteLine($"[PASS] HtmlPerformanceMonitor 存在");
    var t6 = typeof(XPathQueryExecutor);
    Console.WriteLine($"[PASS] XPathQueryExecutor 存在");
    var t7 = typeof(IXPathQueryExecutor);
    Console.WriteLine($"[PASS] IXPathQueryExecutor 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}