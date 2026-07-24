#load "load_balancer.cs"

Console.WriteLine("=== load_balancer Test ===");

try
{
    var t0 = typeof(LoadBalancer);
    Console.WriteLine($"[PASS] LoadBalancer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}