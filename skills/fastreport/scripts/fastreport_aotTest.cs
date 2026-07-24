#load "fastreport_aot.cs"

Console.WriteLine("=== fastreport_aot Test ===");

try
{
    var t0 = typeof(FastReport.AOT.FastReportOptions);
    Console.WriteLine($"[PASS] FastReportOptions 存在");
    var t1 = typeof(FastReport.AOT.ReportParameter);
    Console.WriteLine($"[PASS] ReportParameter 存在");
    var t2 = typeof(FastReport.AOT.ReportData);
    Console.WriteLine($"[PASS] ReportData 存在");
    var t3 = typeof(FastReport.AOT.FastReportCommandResult);
    Console.WriteLine($"[PASS] FastReportCommandResult 存在");
    var t4 = typeof(FastReport.AOT.FastReportService);
    Console.WriteLine($"[PASS] FastReportService 存在");
    var t5 = typeof(FastReport.AOT.IFastReportService);
    Console.WriteLine($"[PASS] IFastReportService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(FastReport.AOT.FastReportCommandType);
    Console.WriteLine($"[PASS] FastReportCommandType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}