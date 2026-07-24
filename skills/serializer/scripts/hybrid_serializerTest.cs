#load "hybrid_serializer.cs"

Console.WriteLine("=== hybrid_serializer Test ===");

try
{
    var t0 = typeof(HybridSerializer);
    Console.WriteLine($"[PASS] HybridSerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}