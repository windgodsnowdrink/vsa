#load "hybrid_protocol_serializer.cs"

Console.WriteLine("=== hybrid_protocol_serializer Test ===");

try
{
    var t0 = typeof(HybridProtocolSerializer);
    Console.WriteLine($"[PASS] HybridProtocolSerializer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}