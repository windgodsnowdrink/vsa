#load "hybrid_json_processor.cs"

Console.WriteLine("=== hybrid_json_processor Test ===");

try
{
    var t0 = typeof(HybridJsonProcessor);
    Console.WriteLine($"[PASS] HybridJsonProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}