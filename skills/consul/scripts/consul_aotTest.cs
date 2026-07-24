#load "consul_aot.cs"

Console.WriteLine("=== consul_aot Test ===");

try
{
    var t0 = typeof(Consul.AOT.ConsulClientOptions);
    Console.WriteLine($"[PASS] ConsulClientOptions 存在");
    var t1 = typeof(Consul.AOT.ConsulService);
    Console.WriteLine($"[PASS] ConsulService 存在");
    var t2 = typeof(Consul.AOT.ConsulAotEngine);
    Console.WriteLine($"[PASS] ConsulAotEngine 存在");
    var t3 = typeof(Consul.AOT.IConsulService);
    Console.WriteLine($"[PASS] IConsulService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}