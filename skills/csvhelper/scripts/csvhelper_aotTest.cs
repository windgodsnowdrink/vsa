#load "csvhelper_aot.cs"

Console.WriteLine("=== csvhelper_aot Test ===");

try
{
    var t0 = typeof(CsvHelper.AOT.CsvHelperOptions);
    Console.WriteLine($"[PASS] CsvHelperOptions 存在");
    var t1 = typeof(CsvHelper.AOT.CsvRecordMap);
    Console.WriteLine($"[PASS] CsvRecordMap 存在");
    var t2 = typeof(CsvHelper.AOT.CsvExportRequest);
    Console.WriteLine($"[PASS] CsvExportRequest 存在");
    var t3 = typeof(CsvHelper.AOT.CsvHelperResult);
    Console.WriteLine($"[PASS] CsvHelperResult 存在");
    var t4 = typeof(CsvHelper.AOT.CsvHelperStatus);
    Console.WriteLine($"[PASS] CsvHelperStatus 存在");
    var t5 = typeof(CsvHelper.AOT.CsvHelperService);
    Console.WriteLine($"[PASS] CsvHelperService 存在");
    var t6 = typeof(CsvHelper.AOT.CsvHelperAotEngine);
    Console.WriteLine($"[PASS] CsvHelperAotEngine 存在");
    var t7 = typeof(CsvHelper.AOT.ICsvHelperService);
    Console.WriteLine($"[PASS] ICsvHelperService 接口存在 (IsInterface: {t7.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}