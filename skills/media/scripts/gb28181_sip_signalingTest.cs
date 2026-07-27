#load "gb28181_sip_signaling.cs"

Console.WriteLine("=== gb28181_sip_signaling Test ===");

try
{
    var t0 = typeof(GB28181SipService);
    Console.WriteLine($"[PASS] GB28181SipService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(IGB28181SipService);
    Console.WriteLine($"[PASS] IGB28181SipService 接口存在 (IsInterface: {t2.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}