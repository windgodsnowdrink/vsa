#load "sharppcap_core.cs"

Console.WriteLine("=== sharppcap_core Test ===");

try
{
    var t0 = typeof(SharpPcapSkill.NetworkInterfaceInfo);
    Console.WriteLine($"[PASS] NetworkInterfaceInfo 存在");
    var t1 = typeof(SharpPcapSkill.PacketInfo);
    Console.WriteLine($"[PASS] PacketInfo 存在");
    var t2 = typeof(SharpPcapSkill.PacketAnalysisResult);
    Console.WriteLine($"[PASS] PacketAnalysisResult 存在");
    var t3 = typeof(SharpPcapSkill.NetworkStats);
    Console.WriteLine($"[PASS] NetworkStats 存在");
    var t4 = typeof(SharpPcapSkill.PacketCaptureService);
    Console.WriteLine($"[PASS] PacketCaptureService 存在");
    var t5 = typeof(SharpPcapSkill.PacketAnalyzerService);
    Console.WriteLine($"[PASS] PacketAnalyzerService 存在");
    var t6 = typeof(SharpPcapSkill.PacketFilterService);
    Console.WriteLine($"[PASS] PacketFilterService 存在");
    var t7 = typeof(SharpPcapSkill.NetworkStatsService);
    Console.WriteLine($"[PASS] NetworkStatsService 存在");
    var t8 = typeof(SharpPcapSkill.SharpPcapCli);
    Console.WriteLine($"[PASS] SharpPcapCli 存在");
    var t9 = typeof(SharpPcapSkill.IPacketCaptureService);
    Console.WriteLine($"[PASS] IPacketCaptureService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(SharpPcapSkill.IPacketAnalyzerService);
    Console.WriteLine($"[PASS] IPacketAnalyzerService 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(SharpPcapSkill.IPacketFilterService);
    Console.WriteLine($"[PASS] IPacketFilterService 接口存在 (IsInterface: {t11.IsInterface})");
    var t12 = typeof(SharpPcapSkill.INetworkStatsService);
    Console.WriteLine($"[PASS] INetworkStatsService 接口存在 (IsInterface: {t12.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}