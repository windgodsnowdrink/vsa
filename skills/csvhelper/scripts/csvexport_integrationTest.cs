#load "csvexport_integration.cs"

Console.WriteLine("=== csvexport_integration Test ===");

try
{
    var t0 = typeof(CsvService);
    Console.WriteLine($"[PASS] CsvService 存在");
    var t1 = typeof(in);
    Console.WriteLine($"[PASS] in record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}