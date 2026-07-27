#load "pdfreport_integration.cs"

Console.WriteLine("=== pdfreport_integration Test ===");

try
{
    var t0 = typeof(PdfReportIntegration.PdfReportOptions);
    Console.WriteLine($"[PASS] PdfReportOptions 存在");
    var t1 = typeof(PdfReportIntegration.PdfReportService);
    Console.WriteLine($"[PASS] PdfReportService 存在");
    var t2 = typeof(PdfReportIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(PdfReportIntegration.IPdfReportService);
    Console.WriteLine($"[PASS] IPdfReportService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}