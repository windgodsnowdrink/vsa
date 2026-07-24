#load "fastreport_integration.cs"

Console.WriteLine("=== fastreport_integration Test ===");

try
{
    var t0 = typeof(FastReportOptions);
    Console.WriteLine($"[PASS] FastReportOptions 存在");
    var t1 = typeof(ReportService);
    Console.WriteLine($"[PASS] ReportService 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(IReportService);
    Console.WriteLine($"[PASS] IReportService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}