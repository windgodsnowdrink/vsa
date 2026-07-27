#load "docker_compose_integration.cs"

Console.WriteLine("=== docker_compose_integration Test ===");

try
{
    var t0 = typeof(DockerComposeIntegration.DockerComposeOptions);
    Console.WriteLine($"[PASS] DockerComposeOptions 存在");
    var t1 = typeof(DockerComposeIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(DockerComposeIntegration.Startup);
    Console.WriteLine($"[PASS] Startup 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}