#load "mediatr_mqtt_integration_message.cs"

Console.WriteLine("=== mediatr_mqtt_integration_message Test ===");

try
{
    var t0 = typeof(MqttPublishBehavior);
    Console.WriteLine($"[PASS] MqttPublishBehavior 存在");
    var t1 = typeof(MqttMessageHandler);
    Console.WriteLine($"[PASS] MqttMessageHandler 存在");
    var t2 = typeof(DistributedTransactionBehavior);
    Console.WriteLine($"[PASS] DistributedTransactionBehavior 存在");
    var t3 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(MqttOptions);
    Console.WriteLine($"[PASS] MqttOptions 存在");
    var t5 = typeof(ProcessMessage);
    Console.WriteLine($"[PASS] ProcessMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}