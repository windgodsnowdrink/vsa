#load "sharppcap_integration.cs"

Console.WriteLine("=== sharppcap_integration Test ===");

try
{
    var t0 = typeof(PacketCaptureOptions);
    Console.WriteLine($"[PASS] PacketCaptureOptions 存在");
    var t1 = typeof(PacketCaptureService);
    Console.WriteLine($"[PASS] PacketCaptureService 存在");
    var t2 = typeof(PacketCaptureExtensions);
    Console.WriteLine($"[PASS] PacketCaptureExtensions 存在");
    var t3 = typeof(IPacketCaptureService);
    Console.WriteLine($"[PASS] IPacketCaptureService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}