#load "mqtt_production_integration.cs"

Console.WriteLine("=== mqtt_production_integration Test ===");

try
{
    var t0 = typeof(MqttProductionIntegration);
    Console.WriteLine($"[PASS] MqttProductionIntegration 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}