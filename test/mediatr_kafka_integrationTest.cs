#load "mediatr_kafka_integration.cs"

Console.WriteLine("=== mediatr_kafka_integration Test ===");

try
{
    var t0 = typeof(KafkaEventBusOptions);
    Console.WriteLine($"[PASS] KafkaEventBusOptions 存在");
    var t1 = typeof(KafkaEventBus);
    Console.WriteLine($"[PASS] KafkaEventBus 存在");
    var t2 = typeof(KafkaHealthCheck);
    Console.WriteLine($"[PASS] KafkaHealthCheck 存在");
    var t3 = typeof(KafkaEventBusExtensions);
    Console.WriteLine($"[PASS] KafkaEventBusExtensions 存在");
    var t4 = typeof(KafkaEventBusBackgroundService);
    Console.WriteLine($"[PASS] KafkaEventBusBackgroundService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}