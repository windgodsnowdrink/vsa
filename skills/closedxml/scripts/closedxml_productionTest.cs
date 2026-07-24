#load "closedxml_production.cs"

Console.WriteLine("=== closedxml_production Test ===");

try
{
    var t0 = typeof(ExcelDocumentService);
    Console.WriteLine($"[PASS] ExcelDocumentService 存在");
    var t1 = typeof(WorkbookPooledPolicy);
    Console.WriteLine($"[PASS] WorkbookPooledPolicy 存在");
    var t2 = typeof(ExcelRequest);
    Console.WriteLine($"[PASS] ExcelRequest record 存在");
    var t3 = typeof(ExcelOperation);
    Console.WriteLine($"[PASS] ExcelOperation record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}