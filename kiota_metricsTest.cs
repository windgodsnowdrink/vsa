#load "kiota_metrics.cs"

Console.WriteLine("=== kiota_metrics Test ===");

try
{
    var t0 = typeof(ChannelCodeGenMonitor);
    Console.WriteLine($"[PASS] ChannelCodeGenMonitor 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}