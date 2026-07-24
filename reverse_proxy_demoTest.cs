#load "reverse_proxy_demo.cs"

Console.WriteLine("=== reverse_proxy_demo Test ===");

try
{
    var t0 = typeof(MemoryOptimizer);
    Console.WriteLine($"[PASS] MemoryOptimizer 存在");
    var t1 = typeof(ProxyConfigService);
    Console.WriteLine($"[PASS] ProxyConfigService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}