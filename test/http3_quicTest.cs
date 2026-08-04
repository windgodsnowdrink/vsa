#load "http3_quic.cs"

Console.WriteLine("=== http3_quic Test ===");

try
{
    var t0 = typeof(Http3Server);
    Console.WriteLine($"[PASS] Http3Server 存在");
    var t1 = typeof(QuicStreamPooledPolicy);
    Console.WriteLine($"[PASS] QuicStreamPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}