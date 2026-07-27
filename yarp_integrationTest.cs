#load "yarp_integration.cs"

Console.WriteLine("=== yarp_integration Test ===");

try
{
    var t0 = typeof(BffGatewayService);
    Console.WriteLine($"[PASS] BffGatewayService 存在");
    var t1 = typeof(ProxyConfigPooledPolicy);
    Console.WriteLine($"[PASS] ProxyConfigPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}