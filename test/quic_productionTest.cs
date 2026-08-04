#load "quic_production.cs"

Console.WriteLine("=== quic_production Test ===");

try
{
    var t0 = typeof(QuicServer);
    Console.WriteLine($"[PASS] QuicServer 存在");
    var t1 = typeof(QuicStreamPooledPolicy);
    Console.WriteLine($"[PASS] QuicStreamPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}