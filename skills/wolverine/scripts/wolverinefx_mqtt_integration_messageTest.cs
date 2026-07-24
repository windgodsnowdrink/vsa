#load "wolverinefx_mqtt_integration_message.cs"

Console.WriteLine("=== wolverinefx_mqtt_integration_message Test ===");

try
{
    var t0 = typeof(MqttMessageHandlers);
    Console.WriteLine($"[PASS] MqttMessageHandlers 存在");
    var t1 = typeof(DistributedTransactionMiddleware);
    Console.WriteLine($"[PASS] DistributedTransactionMiddleware 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(MqttOptions);
    Console.WriteLine($"[PASS] MqttOptions 存在");
    var t4 = typeof(ProcessMessage);
    Console.WriteLine($"[PASS] ProcessMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}