#load "mediatr_mqtt_integration_pubsub.cs"

Console.WriteLine("=== mediatr_mqtt_integration_pubsub Test ===");

try
{
    var t0 = typeof(MqttPublishBehavior);
    Console.WriteLine($"[PASS] MqttPublishBehavior 存在");
    var t1 = typeof(MqttSubscriptionHandler);
    Console.WriteLine($"[PASS] MqttSubscriptionHandler 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(MqttTransactionalBehavior);
    Console.WriteLine($"[PASS] MqttTransactionalBehavior 存在");
    var t4 = typeof(MqttReliabilityService);
    Console.WriteLine($"[PASS] MqttReliabilityService 存在");
    var t5 = typeof(MqttPayloadSerializer);
    Console.WriteLine($"[PASS] MqttPayloadSerializer 存在");
    var t6 = typeof(TruncatedMemoryOwner);
    Console.WriteLine($"[PASS] TruncatedMemoryOwner 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}