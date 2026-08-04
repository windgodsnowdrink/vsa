#load "feedback_loop.cs"

Console.WriteLine("=== feedback_loop Test ===");

try
{
    var t0 = typeof(FeedbackProcessor);
    Console.WriteLine($"[PASS] FeedbackProcessor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}