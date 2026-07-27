#load "jitter_algorithm_production.cs"

Console.WriteLine("=== jitter_algorithm_production Test ===");

try
{
    var t0 = typeof(JitterOptions);
    Console.WriteLine($"[PASS] JitterOptions 存在");
    var t1 = typeof(JitterAlgorithm);
    Console.WriteLine($"[PASS] JitterAlgorithm 存在");
    var t2 = typeof(JitterExtensions);
    Console.WriteLine($"[PASS] JitterExtensions 存在");
    var t3 = typeof(JitterUsageExample);
    Console.WriteLine($"[PASS] JitterUsageExample 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}