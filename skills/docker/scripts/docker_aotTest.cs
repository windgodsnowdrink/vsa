#load "docker_aot.cs"

Console.WriteLine("=== docker_aot Test ===");

try
{
    var t0 = typeof(Docker.AOT.DockerOptions);
    Console.WriteLine($"[PASS] DockerOptions 存在");
    var t1 = typeof(Docker.AOT.DockerCommandResult);
    Console.WriteLine($"[PASS] DockerCommandResult 存在");
    var t2 = typeof(Docker.AOT.DockerService);
    Console.WriteLine($"[PASS] DockerService 存在");
    var t3 = typeof(Docker.AOT.DockerAotEngine);
    Console.WriteLine($"[PASS] DockerAotEngine 存在");
    var t4 = typeof(Docker.AOT.DockerServiceExtensions);
    Console.WriteLine($"[PASS] DockerServiceExtensions 存在");
    var t5 = typeof(Docker.AOT.IDockerService);
    Console.WriteLine($"[PASS] IDockerService 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(Docker.AOT.DockerCommandType);
    Console.WriteLine($"[PASS] DockerCommandType enum 存在 (IsEnum: {t6.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}