#load "mqtt_redis_integration.cs"

Console.WriteLine("=== mqtt_redis_integration Test ===");

try
{
    var t0 = typeof(RedisStreamConsumer);
    Console.WriteLine($"[PASS] RedisStreamConsumer 存在");
    var t1 = typeof(MqttMessage);
    Console.WriteLine($"[PASS] MqttMessage record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}