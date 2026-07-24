#load "closedxml_aot.cs"

Console.WriteLine("=== closedxml_aot Test ===");

try
{
    var t0 = typeof(ClosedXML.AOT.ClosedXmlAotEngine);
    Console.WriteLine($"[PASS] ClosedXmlAotEngine 存在");
    var t1 = typeof(ClosedXML.AOT.ExcelService);
    Console.WriteLine($"[PASS] ExcelService 存在");
    var t2 = typeof(ClosedXML.AOT.IExcelService);
    Console.WriteLine($"[PASS] IExcelService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}