#load "mqtt_biometric_security.cs"

Console.WriteLine("=== mqtt_biometric_security Test ===");

try
{
    var t0 = typeof(AzureKeyVaultService);
    Console.WriteLine($"[PASS] AzureKeyVaultService 存在");
    var t1 = typeof(VoicePrintAuthenticator);
    Console.WriteLine($"[PASS] VoicePrintAuthenticator 存在");
    var t2 = typeof(MLBehaviorAnalyzer);
    Console.WriteLine($"[PASS] MLBehaviorAnalyzer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}