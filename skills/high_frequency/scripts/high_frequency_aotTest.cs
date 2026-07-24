#load "high_frequency_aot.cs"

Console.WriteLine("=== high_frequency_aot Test ===");

try
{
    var t0 = typeof(HighFrequencySettings);
    Console.WriteLine($"[PASS] HighFrequencySettings 存在");
    var t1 = typeof(ProcessingResult);
    Console.WriteLine($"[PASS] ProcessingResult 存在");
    var t2 = typeof(HighFrequencyService);
    Console.WriteLine($"[PASS] HighFrequencyService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}