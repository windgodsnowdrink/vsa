#load "seq_integration.cs"

Console.WriteLine("=== seq_integration Test ===");

try
{
    var t0 = typeof(SeqConfig);
    Console.WriteLine($"[PASS] SeqConfig 存在");
    var t1 = typeof(SeqLogChannel);
    Console.WriteLine($"[PASS] SeqLogChannel 存在");
    var t2 = typeof(LogExtensions);
    Console.WriteLine($"[PASS] LogExtensions 存在");
    var t3 = typeof(SeqBackgroundService);
    Console.WriteLine($"[PASS] SeqBackgroundService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}