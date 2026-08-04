#load "mqtt_federated_learning.cs"

Console.WriteLine("=== mqtt_federated_learning Test ===");

try
{
    var t0 = typeof(EdgeFederatedLearner);
    Console.WriteLine($"[PASS] EdgeFederatedLearner 存在");
    var t1 = typeof(DistributedEdgeComputer);
    Console.WriteLine($"[PASS] DistributedEdgeComputer 存在");
    var t2 = typeof(SidhCryptoProvider);
    Console.WriteLine($"[PASS] SidhCryptoProvider 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}