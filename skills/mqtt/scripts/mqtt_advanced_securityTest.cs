#load "mqtt_advanced_security.cs"

Console.WriteLine("=== mqtt_advanced_security Test ===");

try
{
    var t0 = typeof(TimeBasedKeyRotation);
    Console.WriteLine($"[PASS] TimeBasedKeyRotation 存在");
    var t1 = typeof(TotpAuthenticator);
    Console.WriteLine($"[PASS] TotpAuthenticator 存在");
    var t2 = typeof(AnomalyDetectionEngine);
    Console.WriteLine($"[PASS] AnomalyDetectionEngine 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}