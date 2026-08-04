#load "docker_compose_advanced_integration.cs"

Console.WriteLine("=== docker_compose_advanced_integration Test ===");

try
{
    var t0 = typeof(DockerComposeAdvancedIntegration.DockerComposeOptions);
    Console.WriteLine($"[PASS] DockerComposeOptions 存在");
    var t1 = typeof(DockerComposeAdvancedIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(DockerComposeAdvancedIntegration.DockerComposeMetricsService);
    Console.WriteLine($"[PASS] DockerComposeMetricsService 存在");
    var t3 = typeof(DockerComposeAdvancedIntegration.Startup);
    Console.WriteLine($"[PASS] Startup 存在");
    var t4 = typeof(DockerComposeAdvancedIntegration.DockerComposeEnvironment);
    Console.WriteLine($"[PASS] DockerComposeEnvironment record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}