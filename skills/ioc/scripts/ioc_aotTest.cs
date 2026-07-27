#load "ioc_aot.cs"

Console.WriteLine("=== ioc_aot Test ===");

try
{
    var t0 = typeof(IocSettings);
    Console.WriteLine($"[PASS] IocSettings 存在");
    var t1 = typeof(BenchmarkResult);
    Console.WriteLine($"[PASS] BenchmarkResult 存在");
    var t2 = typeof(IocContainer);
    Console.WriteLine($"[PASS] IocContainer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}