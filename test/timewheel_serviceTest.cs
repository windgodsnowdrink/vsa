#load "timewheel_service.cs"

Console.WriteLine("=== timewheel_service Test ===");

try
{
    var t0 = typeof(TimeWheel);
    Console.WriteLine($"[PASS] TimeWheel 存在");
    var t1 = typeof(TimeTask);
    Console.WriteLine($"[PASS] TimeTask 存在");
    var t2 = typeof(TimeTaskPooledPolicy);
    Console.WriteLine($"[PASS] TimeTaskPooledPolicy 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}