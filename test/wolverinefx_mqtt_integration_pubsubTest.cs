#load "wolverinefx_mqtt_integration_pubsub.cs"

Console.WriteLine("=== wolverinefx_mqtt_integration_pubsub Test ===");

try
{
    var t0 = typeof(MqttMessageHandlers);
    Console.WriteLine($"[PASS] MqttMessageHandlers 存在");
    var t1 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t2 = typeof(MqttTransport);
    Console.WriteLine($"[PASS] MqttTransport 存在");
    var t3 = typeof(MqttTransactionalMiddleware);
    Console.WriteLine($"[PASS] MqttTransactionalMiddleware 存在");
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