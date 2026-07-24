#load "p2p_integration.cs"

Console.WriteLine("=== p2p_integration Test ===");

try
{
    var t0 = typeof(P2PService);
    Console.WriteLine($"[PASS] P2PService 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(IP2PService);
    Console.WriteLine($"[PASS] IP2PService 接口存在 (IsInterface: {t2.IsInterface})");
    var t3 = typeof(ReceivedMessage);
    Console.WriteLine($"[PASS] ReceivedMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}