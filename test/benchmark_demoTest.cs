#load "benchmark_demo.cs"

Console.WriteLine("=== benchmark_demo Test ===");

try
{
    var t0 = typeof(TodoItem);
    Console.WriteLine($"[PASS] TodoItem 存在");
    var t1 = typeof(TodoDbContext);
    Console.WriteLine($"[PASS] TodoDbContext 存在");
    var t2 = typeof(TodoBenchmark);
    Console.WriteLine($"[PASS] TodoBenchmark 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}