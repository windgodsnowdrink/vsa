#load "kcp_transport.cs"

Console.WriteLine("=== kcp_transport Test ===");

try
{
    var t0 = typeof(KcpMultiplexer);
    Console.WriteLine($"[PASS] KcpMultiplexer 存在");
    var t1 = typeof(KcpStreamPooledPolicy);
    Console.WriteLine($"[PASS] KcpStreamPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}