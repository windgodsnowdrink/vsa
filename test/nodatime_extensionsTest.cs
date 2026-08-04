#load "nodatime_extensions.cs"

Console.WriteLine("=== nodatime_extensions Test ===");

try
{
    var t0 = typeof(NodaTimeService);
    Console.WriteLine($"[PASS] NodaTimeService 存在");
    var t1 = typeof(NodaTimeExtensions);
    Console.WriteLine($"[PASS] NodaTimeExtensions 存在");
    var t2 = typeof(TimeRequest);
    Console.WriteLine($"[PASS] TimeRequest record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}