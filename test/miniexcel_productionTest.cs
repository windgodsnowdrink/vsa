#load "miniexcel_production.cs"

Console.WriteLine("=== miniexcel_production Test ===");

try
{
    var t0 = typeof(ExcelGenerationService);
    Console.WriteLine($"[PASS] ExcelGenerationService 存在");
    var t1 = typeof(MemoryStreamPooledPolicy);
    Console.WriteLine($"[PASS] MemoryStreamPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}