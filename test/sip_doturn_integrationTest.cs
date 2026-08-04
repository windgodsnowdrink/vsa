#load "sip_doturn_integration.cs"

Console.WriteLine("=== sip_doturn_integration Test ===");

try
{
    var t0 = typeof(TurnIntegration);
    Console.WriteLine($"[PASS] TurnIntegration 存在");
    var t1 = typeof(SIPSignalingEngine);
    Console.WriteLine($"[PASS] SIPSignalingEngine 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}