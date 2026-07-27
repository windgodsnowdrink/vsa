#load "excel_aot.cs"

Console.WriteLine("=== excel_aot Test ===");

try
{
    var t0 = typeof(Excel.AOT.ExcelOptions);
    Console.WriteLine($"[PASS] ExcelOptions 存在");
    var t1 = typeof(Excel.AOT.ExcelCell);
    Console.WriteLine($"[PASS] ExcelCell 存在");
    var t2 = typeof(Excel.AOT.ExcelWorksheet);
    Console.WriteLine($"[PASS] ExcelWorksheet 存在");
    var t3 = typeof(Excel.AOT.ExcelDocument);
    Console.WriteLine($"[PASS] ExcelDocument 存在");
    var t4 = typeof(Excel.AOT.ExcelCommandResult);
    Console.WriteLine($"[PASS] ExcelCommandResult 存在");
    var t5 = typeof(Excel.AOT.ExcelService);
    Console.WriteLine($"[PASS] ExcelService 存在");
    var t6 = typeof(Excel.AOT.IExcelService);
    Console.WriteLine($"[PASS] IExcelService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(Excel.AOT.ExcelCommandType);
    Console.WriteLine($"[PASS] ExcelCommandType enum 存在 (IsEnum: {t7.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}