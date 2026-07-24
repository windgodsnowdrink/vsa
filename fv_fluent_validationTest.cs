#load "fv_fluent_validation.cs"

Console.WriteLine("=== fv_fluent_validation Test ===");

try
{
    var t0 = typeof(StreamingValidator);
    Console.WriteLine($"[PASS] StreamingValidator 存在");
    var t1 = typeof(PooledStreamingValidator);
    Console.WriteLine($"[PASS] PooledStreamingValidator 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}