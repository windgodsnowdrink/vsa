#load "cellreport_integration.cs"

Console.WriteLine("=== cellreport_integration Test ===");

try
{
    var t0 = typeof(YourNamespace.CellReportExportService);
    Console.WriteLine($"[PASS] CellReportExportService 存在");
    var t1 = typeof(YourNamespace.CellReportDataSourceProvider);
    Console.WriteLine($"[PASS] CellReportDataSourceProvider 存在");
    var t2 = typeof(YourNamespace.CellReportTemplateManager);
    Console.WriteLine($"[PASS] CellReportTemplateManager 存在");
    var t3 = typeof(YourNamespace.CellReportOptions);
    Console.WriteLine($"[PASS] CellReportOptions 存在");
    var t4 = typeof(YourNamespace.CellReportServiceExtensions);
    Console.WriteLine($"[PASS] CellReportServiceExtensions 存在");
    var t5 = typeof(YourNamespace.CellReportServiceCollectionExtensions);
    Console.WriteLine($"[PASS] CellReportServiceCollectionExtensions 存在");
    var t6 = typeof(YourNamespace.CellReportEnginePooledObjectPolicy);
    Console.WriteLine($"[PASS] CellReportEnginePooledObjectPolicy 存在");
    var t7 = typeof(YourNamespace.CellReportEngine);
    Console.WriteLine($"[PASS] CellReportEngine 存在");
    var t8 = typeof(YourNamespace.CellReportRenderService);
    Console.WriteLine($"[PASS] CellReportRenderService 存在");
    var t9 = typeof(YourNamespace.CellReportRenderJob);
    Console.WriteLine($"[PASS] CellReportRenderJob record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}