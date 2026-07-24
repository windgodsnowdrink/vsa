#load "masstransit_mqttnet_integration.cs"

Console.WriteLine("=== masstransit_mqttnet_integration Test ===");

try
{
    var t0 = typeof(CustomMqttTransport);
    Console.WriteLine($"[PASS] CustomMqttTransport 存在");
    var t1 = typeof(MqttMessageConsumer);
    Console.WriteLine($"[PASS] MqttMessageConsumer 存在");
    var t2 = typeof(RabbitToMqttBridgeConsumer);
    Console.WriteLine($"[PASS] RabbitToMqttBridgeConsumer 存在");
    var t3 = typeof(MqttToRabbitBridgeConsumer);
    Console.WriteLine($"[PASS] MqttToRabbitBridgeConsumer 存在");
    var t4 = typeof(MqttMessage);
    Console.WriteLine($"[PASS] MqttMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}