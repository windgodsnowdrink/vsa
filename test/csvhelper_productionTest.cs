#load "csvhelper_production.cs"

Console.WriteLine("=== csvhelper_production Test ===");

try
{
    var t0 = typeof(CsvProcessingService);
    Console.WriteLine($"[PASS] CsvProcessingService 存在");
    var t1 = typeof(in);
    Console.WriteLine($"[PASS] in record 存在");
    var t2 = typeof(CsvOperation);
    Console.WriteLine($"[PASS] CsvOperation record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}