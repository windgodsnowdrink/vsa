#load "signalr_monitoring.cs"

Console.WriteLine("=== signalr_monitoring Test ===");

try
{
    var t0 = typeof(CompressionMonitor);
    Console.WriteLine($"[PASS] CompressionMonitor 存在");
    var t1 = typeof(ExpirationEventNotifier);
    Console.WriteLine($"[PASS] ExpirationEventNotifier 存在");
    var t2 = typeof(HotUpdateQoSConfig);
    Console.WriteLine($"[PASS] HotUpdateQoSConfig 存在");
    var t3 = typeof(QoSOptions);
    Console.WriteLine($"[PASS] QoSOptions 存在");
    var t4 = typeof(CompressionMetrics);
    Console.WriteLine($"[PASS] CompressionMetrics record 存在");
    var t5 = typeof(ExpiredMessageEvent);
    Console.WriteLine($"[PASS] ExpiredMessageEvent record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}