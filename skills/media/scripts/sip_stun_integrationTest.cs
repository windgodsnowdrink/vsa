#load "sip_stun_integration.cs"

Console.WriteLine("=== sip_stun_integration Test ===");

try
{
    var t0 = typeof(STUNClient);
    Console.WriteLine($"[PASS] STUNClient 存在");
    var t1 = typeof(SIPSignalingEngine);
    Console.WriteLine($"[PASS] SIPSignalingEngine 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}