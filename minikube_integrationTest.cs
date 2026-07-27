#load "minikube_integration.cs"

Console.WriteLine("=== minikube_integration Test ===");

try
{
    var t0 = typeof(MinikubeIntegration);
    Console.WriteLine($"[PASS] MinikubeIntegration 存在");
    var t1 = typeof(MinikubeOptions);
    Console.WriteLine($"[PASS] MinikubeOptions 存在");
    var t2 = typeof(MinikubeDeploymentManager);
    Console.WriteLine($"[PASS] MinikubeDeploymentManager 存在");
    var t3 = typeof(MinikubeServiceDiscovery);
    Console.WriteLine($"[PASS] MinikubeServiceDiscovery 存在");
    var t4 = typeof(MinikubeConfigManager);
    Console.WriteLine($"[PASS] MinikubeConfigManager 存在");
    var t5 = typeof(DeploymentPooledPolicy);
    Console.WriteLine($"[PASS] DeploymentPooledPolicy 存在");
    var t6 = typeof(MinikubeTenantManager);
    Console.WriteLine($"[PASS] MinikubeTenantManager 存在");
    var t7 = typeof(MinikubePerformanceMonitor);
    Console.WriteLine($"[PASS] MinikubePerformanceMonitor 存在");
    var t8 = typeof(MinikubeTestRunner);
    Console.WriteLine($"[PASS] MinikubeTestRunner 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}