#load "mqtt_quantum_security.cs"

Console.WriteLine("=== mqtt_quantum_security Test ===");

try
{
    var t0 = typeof(GaussianNoiseGenerator);
    Console.WriteLine($"[PASS] GaussianNoiseGenerator 存在");
    var t1 = typeof(AzureIoTEdgeManager);
    Console.WriteLine($"[PASS] AzureIoTEdgeManager 存在");
    var t2 = typeof(BB84Protocol);
    Console.WriteLine($"[PASS] BB84Protocol 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}