#load "htmlsanitizer_integration.cs"

Console.WriteLine("=== htmlsanitizer_integration Test ===");

try
{
    var t0 = typeof(HtmlSanitizerIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t1 = typeof(HtmlSanitizerIntegration.HtmlSanitizerOptions);
    Console.WriteLine($"[PASS] HtmlSanitizerOptions 存在");
    var t2 = typeof(HtmlSanitizerIntegration.HtmlProcessingService);
    Console.WriteLine($"[PASS] HtmlProcessingService 存在");
    var t3 = typeof(HtmlSanitizerIntegration.HtmlSanitizerPooledObjectPolicy);
    Console.WriteLine($"[PASS] HtmlSanitizerPooledObjectPolicy 存在");
    var t4 = typeof(HtmlSanitizerIntegration.RemoveIframesStrategy);
    Console.WriteLine($"[PASS] RemoveIframesStrategy 存在");
    var t5 = typeof(HtmlSanitizerIntegration.ICustomSanitizerStrategy);
    Console.WriteLine($"[PASS] ICustomSanitizerStrategy 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(HtmlSanitizerIntegration.HtmlProcessingJob);
    Console.WriteLine($"[PASS] HtmlProcessingJob record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}