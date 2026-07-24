#load "SourceGenerator_fody_mqtt_test.cs"

Console.WriteLine("=== SourceGenerator_fody_mqtt_test Test ===");

try
{
    var t0 = typeof(SourceGenerator.Fody.Mqtt.FodyConfiguration);
    Console.WriteLine($"[PASS] FodyConfiguration 存在");
    var t1 = typeof(SourceGenerator.Fody.Mqtt.MqttEventBus);
    Console.WriteLine($"[PASS] MqttEventBus 存在");
    var t2 = typeof(SourceGenerator.Fody.Mqtt.MqttEventBusTests);
    Console.WriteLine($"[PASS] MqttEventBusTests 存在");
    var t3 = typeof(SourceGenerator.Fody.Mqtt.IEventBus);
    Console.WriteLine($"[PASS] IEventBus 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}