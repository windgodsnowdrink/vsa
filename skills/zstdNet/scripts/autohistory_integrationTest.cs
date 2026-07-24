#load "autohistory_integration.cs"

Console.WriteLine("=== autohistory_integration Test ===");

try
{
    var t0 = typeof(AppDbContext);
    Console.WriteLine($"[PASS] AppDbContext 存在");
    var t1 = typeof(CompressedAutoHistory);
    Console.WriteLine($"[PASS] CompressedAutoHistory 存在");
    var t2 = typeof(HistoryCompressionService);
    Console.WriteLine($"[PASS] HistoryCompressionService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}