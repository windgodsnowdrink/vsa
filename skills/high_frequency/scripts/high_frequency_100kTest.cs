#load "high_frequency_100k.cs"

Console.WriteLine("=== high_frequency_100k Test ===");

try
{
    var t0 = typeof(ZeroCopySqlProcessor);
    Console.WriteLine($"[PASS] ZeroCopySqlProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}