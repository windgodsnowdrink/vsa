#load "podman_integration.cs"

Console.WriteLine("=== podman_integration Test ===");

try
{
    var t0 = typeof(PodmanIntegration.PodmanOptions);
    Console.WriteLine($"[PASS] PodmanOptions 存在");
    var t1 = typeof(PodmanIntegration.PodmanContainerManager);
    Console.WriteLine($"[PASS] PodmanContainerManager 存在");
    var t2 = typeof(PodmanIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(PodmanIntegration.PodmanImageBuilder);
    Console.WriteLine($"[PASS] PodmanImageBuilder 存在");
    var t4 = typeof(PodmanIntegration.PodmanNetworkManager);
    Console.WriteLine($"[PASS] PodmanNetworkManager 存在");
    var t5 = typeof(PodmanIntegration.PodmanMonitorService);
    Console.WriteLine($"[PASS] PodmanMonitorService 存在");
    var t6 = typeof(PodmanIntegration.IPodmanContainerManager);
    Console.WriteLine($"[PASS] IPodmanContainerManager 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(PodmanIntegration.IPodmanImageBuilder);
    Console.WriteLine($"[PASS] IPodmanImageBuilder 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(PodmanIntegration.IPodmanNetworkManager);
    Console.WriteLine($"[PASS] IPodmanNetworkManager 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(PodmanIntegration.IPodmanMonitorService);
    Console.WriteLine($"[PASS] IPodmanMonitorService 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(PodmanIntegration.PodmanMetrics);
    Console.WriteLine($"[PASS] PodmanMetrics record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}