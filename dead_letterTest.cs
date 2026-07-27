#load "dead_letter.cs"

Console.WriteLine("=== dead_letter Test ===");

try
{
    var t0 = typeof(DeadLetterQueue);
    Console.WriteLine($"[PASS] DeadLetterQueue 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}