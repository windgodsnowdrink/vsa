#load "docker_integration.cs"

Console.WriteLine("=== docker_integration Test ===");

try
{
    var t0 = typeof(DockerIntegration.DockerServiceCollectionExtensions);
    Console.WriteLine($"[PASS] DockerServiceCollectionExtensions 存在");
    var t1 = typeof(DockerIntegration.DockerOptions);
    Console.WriteLine($"[PASS] DockerOptions 存在");
    var t2 = typeof(DockerIntegration.DockerContainerManager);
    Console.WriteLine($"[PASS] DockerContainerManager 存在");
    var t3 = typeof(DockerIntegration.DockerImageBuilder);
    Console.WriteLine($"[PASS] DockerImageBuilder 存在");
    var t4 = typeof(DockerIntegration.DockerNetworkManager);
    Console.WriteLine($"[PASS] DockerNetworkManager 存在");
    var t5 = typeof(DockerIntegration.DockerMonitorService);
    Console.WriteLine($"[PASS] DockerMonitorService 存在");
    var t6 = typeof(DockerIntegration.DockerLogCollector);
    Console.WriteLine($"[PASS] DockerLogCollector 存在");
    var t7 = typeof(DockerIntegration.DockerHealthChecker);
    Console.WriteLine($"[PASS] DockerHealthChecker 存在");
    var t8 = typeof(DockerIntegration.ContainerEvent);
    Console.WriteLine($"[PASS] ContainerEvent record 存在");
    var t9 = typeof(DockerIntegration.ContainerMetrics);
    Console.WriteLine($"[PASS] ContainerMetrics record 存在");
    var t10 = typeof(DockerIntegration.HealthCheckResult);
    Console.WriteLine($"[PASS] HealthCheckResult record 存在");
    var t11 = typeof(DockerIntegration.ContainerEventType);
    Console.WriteLine($"[PASS] ContainerEventType enum 存在 (IsEnum: {t11.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}