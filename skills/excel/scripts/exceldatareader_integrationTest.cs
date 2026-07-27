#load "exceldatareader_integration.cs"

Console.WriteLine("=== exceldatareader_integration Test ===");

try
{
    var t0 = typeof(ExcelDataService);
    Console.WriteLine($"[PASS] ExcelDataService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}