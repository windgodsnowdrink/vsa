#load "mqtt_optimized.cs"

Console.WriteLine("=== mqtt_optimized Test ===");

try
{
    var t0 = typeof(RingBufferMessageQueue);
    Console.WriteLine($"[PASS] RingBufferMessageQueue 存在");
    var t1 = typeof(MqttMessageEvent);
    Console.WriteLine($"[PASS] MqttMessageEvent 存在");
    var t2 = typeof(MqttMessageHandler);
    Console.WriteLine($"[PASS] MqttMessageHandler 存在");
    var t3 = typeof(LZ4MessageCompressor);
    Console.WriteLine($"[PASS] LZ4MessageCompressor 存在");
    var t4 = typeof(TokenBucketTrafficController);
    Console.WriteLine($"[PASS] TokenBucketTrafficController 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}