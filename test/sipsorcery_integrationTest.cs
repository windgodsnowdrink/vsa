#load "sipsorcery_integration.cs"

Console.WriteLine("=== sipsorcery_integration Test ===");

try
{
    var t0 = typeof(SIPSignalingEngine);
    Console.WriteLine($"[PASS] SIPSignalingEngine 存在");
    var t1 = typeof(SIPZeroCopyTransport);
    Console.WriteLine($"[PASS] SIPZeroCopyTransport 存在");
    var t2 = typeof(SIPEventProcessor);
    Console.WriteLine($"[PASS] SIPEventProcessor 存在");
    var t3 = typeof(WebRTCIntegration);
    Console.WriteLine($"[PASS] WebRTCIntegration 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}