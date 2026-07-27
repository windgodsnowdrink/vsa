#load "SourceGenerator_fody_mqtt_test.cs"

Console.WriteLine("=== SourceGenerator_fody_mqtt_test Test ===");

try
{
    var t0 = typeof(FodyConfiguration);
    Console.WriteLine($"[PASS] FodyConfiguration 存在");
    var t1 = typeof(MqttEventBus);
    Console.WriteLine($"[PASS] MqttEventBus 存在");
    var t2 = typeof(MqttEventBusTests);
    Console.WriteLine($"[PASS] MqttEventBusTests 存在");
    var t3 = typeof(MqttIntegrationTests);
    Console.WriteLine($"[PASS] MqttIntegrationTests 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}