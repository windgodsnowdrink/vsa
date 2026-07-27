#load "sharppcap_advanced_integration.cs"

Console.WriteLine("=== sharppcap_advanced_integration Test ===");

try
{
    var t0 = typeof(AdvancedPacketCaptureOptions);
    Console.WriteLine($"[PASS] AdvancedPacketCaptureOptions 存在");
    var t1 = typeof(AdvancedPacketCaptureService);
    Console.WriteLine($"[PASS] AdvancedPacketCaptureService 存在");
    var t2 = typeof(AdvancedPacketCaptureExtensions);
    Console.WriteLine($"[PASS] AdvancedPacketCaptureExtensions 存在");
    var t3 = typeof(IAdvancedPacketCaptureService);
    Console.WriteLine($"[PASS] IAdvancedPacketCaptureService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}