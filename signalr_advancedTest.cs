#load "signalr_advanced.cs"

Console.WriteLine("=== signalr_advanced Test ===");

try
{
    var t0 = typeof(AdvancedHub);
    Console.WriteLine($"[PASS] AdvancedHub 存在");
    var t1 = typeof(PriorityBasedQoS);
    Console.WriteLine($"[PASS] PriorityBasedQoS 存在");
    var t2 = typeof(OfflineMessageContext);
    Console.WriteLine($"[PASS] OfflineMessageContext 存在");
    var t3 = typeof(QoSEntry);
    Console.WriteLine($"[PASS] QoSEntry record 存在");
    var t4 = typeof(OfflineMessage);
    Console.WriteLine($"[PASS] OfflineMessage record 存在");
    var t5 = typeof(QoSPriority);
    Console.WriteLine($"[PASS] QoSPriority enum 存在 (IsEnum: {t5.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}