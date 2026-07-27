#load "mqtt_multimodal_security.cs"

Console.WriteLine("=== mqtt_multimodal_security Test ===");

try
{
    var t0 = typeof(BioMetricAuthenticator);
    Console.WriteLine($"[PASS] BioMetricAuthenticator 存在");
    var t1 = typeof(AdaptiveRiskScorer);
    Console.WriteLine($"[PASS] AdaptiveRiskScorer 存在");
    var t2 = typeof(DynamicPolicyEngine);
    Console.WriteLine($"[PASS] DynamicPolicyEngine 存在");
    var t3 = typeof(SecurityLevel);
    Console.WriteLine($"[PASS] SecurityLevel enum 存在 (IsEnum: {t3.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}