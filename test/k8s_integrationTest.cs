#load "k8s_integration.cs"

Console.WriteLine("=== k8s_integration Test ===");

try
{
    var t0 = typeof(KubernetesIntegrationOptions);
    Console.WriteLine($"[PASS] KubernetesIntegrationOptions 存在");
    var t1 = typeof(KubernetesServiceCollectionExtensions);
    Console.WriteLine($"[PASS] KubernetesServiceCollectionExtensions 存在");
    var t2 = typeof(KubernetesDeploymentManager);
    Console.WriteLine($"[PASS] KubernetesDeploymentManager 存在");
    var t3 = typeof(KubernetesServiceDiscovery);
    Console.WriteLine($"[PASS] KubernetesServiceDiscovery 存在");
    var t4 = typeof(KubernetesConfigManager);
    Console.WriteLine($"[PASS] KubernetesConfigManager 存在");
    var t5 = typeof(KubernetesCrdManager);
    Console.WriteLine($"[PASS] KubernetesCrdManager 存在");
    var t6 = typeof(KubernetesAutoScaler);
    Console.WriteLine($"[PASS] KubernetesAutoScaler 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}