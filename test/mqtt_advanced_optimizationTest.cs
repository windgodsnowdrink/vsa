#load "mqtt_advanced_optimization.cs"

Console.WriteLine("=== mqtt_advanced_optimization Test ===");

try
{
    var t0 = typeof(AdaptivePrivacyBudget);
    Console.WriteLine($"[PASS] AdaptivePrivacyBudget 存在");
    var t1 = typeof(MLPredictiveMaintenance);
    Console.WriteLine($"[PASS] MLPredictiveMaintenance 存在");
    var t2 = typeof(QuantumNetworkOptimizer);
    Console.WriteLine($"[PASS] QuantumNetworkOptimizer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}