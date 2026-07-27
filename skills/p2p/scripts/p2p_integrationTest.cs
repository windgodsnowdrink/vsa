#load "p2p_integration.cs"

Console.WriteLine("=== p2p_integration Test ===");

try
{
    var t0 = typeof(P2P.P2POptions);
    Console.WriteLine($"[PASS] P2POptions 存在");
    var t1 = typeof(P2P.P2PNode);
    Console.WriteLine($"[PASS] P2PNode 存在");
    var t2 = typeof(P2P.P2PFile);
    Console.WriteLine($"[PASS] P2PFile 存在");
    var t3 = typeof(P2P.P2PMessage);
    Console.WriteLine($"[PASS] P2PMessage 存在");
    var t4 = typeof(P2P.P2PNetworkStatus);
    Console.WriteLine($"[PASS] P2PNetworkStatus 存在");
    var t5 = typeof(P2P.P2PFileTransferStatus);
    Console.WriteLine($"[PASS] P2PFileTransferStatus 存在");
    var t6 = typeof(P2P.P2PNetworkTopology);
    Console.WriteLine($"[PASS] P2PNetworkTopology 存在");
    var t7 = typeof(P2P.P2PConnection);
    Console.WriteLine($"[PASS] P2PConnection 存在");
    var t8 = typeof(P2P.P2PService);
    Console.WriteLine($"[PASS] P2PService 存在");
    var t9 = typeof(P2P.P2PFileSharingService);
    Console.WriteLine($"[PASS] P2PFileSharingService 存在");
    var t10 = typeof(P2P.P2PMessagingService);
    Console.WriteLine($"[PASS] P2PMessagingService 存在");
    var t11 = typeof(P2P.P2PNetworkService);
    Console.WriteLine($"[PASS] P2PNetworkService 存在");
    var t12 = typeof(P2P.P2PSecurityService);
    Console.WriteLine($"[PASS] P2PSecurityService 存在");
    var t13 = typeof(P2P.P2PServiceCollectionExtensions);
    Console.WriteLine($"[PASS] P2PServiceCollectionExtensions 存在");
    var t14 = typeof(P2P.IP2PService);
    Console.WriteLine($"[PASS] IP2PService 接口存在 (IsInterface: {t14.IsInterface})");
    var t15 = typeof(P2P.IP2PFileSharingService);
    Console.WriteLine($"[PASS] IP2PFileSharingService 接口存在 (IsInterface: {t15.IsInterface})");
    var t16 = typeof(P2P.IP2PMessagingService);
    Console.WriteLine($"[PASS] IP2PMessagingService 接口存在 (IsInterface: {t16.IsInterface})");
    var t17 = typeof(P2P.IP2PNetworkService);
    Console.WriteLine($"[PASS] IP2PNetworkService 接口存在 (IsInterface: {t17.IsInterface})");
    var t18 = typeof(P2P.IP2PSecurityService);
    Console.WriteLine($"[PASS] IP2PSecurityService 接口存在 (IsInterface: {t18.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}