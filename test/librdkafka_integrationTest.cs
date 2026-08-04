#load "librdkafka_integration.cs"

Console.WriteLine("=== librdkafka_integration Test ===");

try
{
    var t0 = typeof(Trae.Vsa.Kafka.KafkaOptions);
    Console.WriteLine($"[PASS] KafkaOptions 存在");
    var t1 = typeof(Trae.Vsa.Kafka.KafkaService);
    Console.WriteLine($"[PASS] KafkaService 存在");
    var t2 = typeof(Trae.Vsa.Kafka.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(Trae.Vsa.Kafka.MemoryPoolStatistics);
    Console.WriteLine($"[PASS] MemoryPoolStatistics 存在");
    var t4 = typeof(Trae.Vsa.Kafka.ConnectionStatistics);
    Console.WriteLine($"[PASS] ConnectionStatistics 存在");
    var t5 = typeof(Trae.Vsa.Kafka.ThroughputStatistics);
    Console.WriteLine($"[PASS] ThroughputStatistics 存在");
    var t6 = typeof(Trae.Vsa.Kafka.IKafkaService);
    Console.WriteLine($"[PASS] IKafkaService 接口存在 (IsInterface: {t6.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}