#load "k3s_integration.cs"

Console.WriteLine("=== k3s_integration Test ===");

try
{
    var t0 = typeof(KubernetesDeploymentManager);
    Console.WriteLine($"[PASS] KubernetesDeploymentManager 存在");
    var t1 = typeof(KubernetesServiceDiscovery);
    Console.WriteLine($"[PASS] KubernetesServiceDiscovery 存在");
    var t2 = typeof(KubernetesConfigManager);
    Console.WriteLine($"[PASS] KubernetesConfigManager 存在");
    var t3 = typeof(K3SOptions);
    Console.WriteLine($"[PASS] K3SOptions 存在");
    var t4 = typeof(K3SExtensions);
    Console.WriteLine($"[PASS] K3SExtensions 存在");
    var t5 = typeof(K3SDeploymentManager);
    Console.WriteLine($"[PASS] K3SDeploymentManager 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}