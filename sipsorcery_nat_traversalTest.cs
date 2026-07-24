#load "sipsorcery_nat_traversal.cs"

Console.WriteLine("=== sipsorcery_nat_traversal Test ===");

try
{
    var t0 = typeof(ICECandidateCollector);
    Console.WriteLine($"[PASS] ICECandidateCollector 存在");
    var t1 = typeof(STUNClient);
    Console.WriteLine($"[PASS] STUNClient 存在");
    var t2 = typeof(NATTraversalService);
    Console.WriteLine($"[PASS] NATTraversalService 存在");
    var t3 = typeof(ClusterTraversalService);
    Console.WriteLine($"[PASS] ClusterTraversalService 存在");
    var t4 = typeof(ClusterTraversalEngine);
    Console.WriteLine($"[PASS] ClusterTraversalEngine 存在");
    var t5 = typeof(EnhancedHealthChecker);
    Console.WriteLine($"[PASS] EnhancedHealthChecker 存在");
    var t6 = typeof(ClusterTraversalConfig);
    Console.WriteLine($"[PASS] ClusterTraversalConfig record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}