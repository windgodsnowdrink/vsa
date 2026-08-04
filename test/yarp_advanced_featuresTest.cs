#load "yarp_advanced_features.cs"

Console.WriteLine("=== yarp_advanced_features Test ===");

try
{
    var t0 = typeof(RoundRobinLoadBalancingPolicy);
    Console.WriteLine($"[PASS] RoundRobinLoadBalancingPolicy 存在");
    var t1 = typeof(WeightedLoadBalancingPolicy);
    Console.WriteLine($"[PASS] WeightedLoadBalancingPolicy 存在");
    var t2 = typeof(SsoClient);
    Console.WriteLine($"[PASS] SsoClient 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}