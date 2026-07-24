#load "sip_coturn_integration.cs"

Console.WriteLine("=== sip_coturn_integration Test ===");

try
{
    var t0 = typeof(CoturnClient);
    Console.WriteLine($"[PASS] CoturnClient 存在");
    var t1 = typeof(SIPSignalingEngine);
    Console.WriteLine($"[PASS] SIPSignalingEngine 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}