#load "miniexcel_advanced.cs"

Console.WriteLine("=== miniexcel_advanced Test ===");

try
{
    var t0 = typeof(AdvancedExcelService);
    Console.WriteLine($"[PASS] AdvancedExcelService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}