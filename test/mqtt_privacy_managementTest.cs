#load "mqtt_privacy_management.cs"

Console.WriteLine("=== mqtt_privacy_management Test ===");

try
{
    var t0 = typeof(DifferentialPrivacyBudget);
    Console.WriteLine($"[PASS] DifferentialPrivacyBudget 存在");
    var t1 = typeof(EdgeDeviceMonitor);
    Console.WriteLine($"[PASS] EdgeDeviceMonitor 存在");
    var t2 = typeof(QuantumRepeaterNode);
    Console.WriteLine($"[PASS] QuantumRepeaterNode 存在");
    var t3 = typeof(HealthStatus);
    Console.WriteLine($"[PASS] HealthStatus enum 存在 (IsEnum: {t3.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}