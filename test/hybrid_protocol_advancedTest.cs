#load "hybrid_protocol_advanced.cs"

Console.WriteLine("=== hybrid_protocol_advanced Test ===");

try
{
    var t0 = typeof(HybridProtocolAdvanced);
    Console.WriteLine($"[PASS] HybridProtocolAdvanced 存在");
    var t1 = typeof(SerializationProtocol);
    Console.WriteLine($"[PASS] SerializationProtocol enum 存在 (IsEnum: {t1.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}