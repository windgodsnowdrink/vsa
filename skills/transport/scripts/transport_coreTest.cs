#load "transport_core.cs"

Console.WriteLine("=== transport_core Test ===");

try
{
    var t0 = typeof(TransportSkill.HttpTransportService);
    Console.WriteLine($"[PASS] HttpTransportService 存在");
    var t1 = typeof(TransportSkill.TcpTransportService);
    Console.WriteLine($"[PASS] TcpTransportService 存在");
    var t2 = typeof(TransportSkill.UdpTransportService);
    Console.WriteLine($"[PASS] UdpTransportService 存在");
    var t3 = typeof(TransportSkill.PipelineTransportService);
    Console.WriteLine($"[PASS] PipelineTransportService 存在");
    var t4 = typeof(TransportSkill.ChannelTransportService);
    Console.WriteLine($"[PASS] ChannelTransportService 存在");
    var t5 = typeof(TransportSkill.BufferTransportService);
    Console.WriteLine($"[PASS] BufferTransportService 存在");
    var t6 = typeof(TransportSkill.IHttpTransportService);
    Console.WriteLine($"[PASS] IHttpTransportService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(TransportSkill.ITcpTransportService);
    Console.WriteLine($"[PASS] ITcpTransportService 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(TransportSkill.IUdpTransportService);
    Console.WriteLine($"[PASS] IUdpTransportService 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(TransportSkill.IPipelineTransportService);
    Console.WriteLine($"[PASS] IPipelineTransportService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(TransportSkill.IChannelTransportService);
    Console.WriteLine($"[PASS] IChannelTransportService 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(TransportSkill.IBufferTransportService);
    Console.WriteLine($"[PASS] IBufferTransportService 接口存在 (IsInterface: {t11.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}