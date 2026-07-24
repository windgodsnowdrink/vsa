#load "kafka_aot.cs"

Console.WriteLine("=== kafka_aot Test ===");

try
{
    var t0 = typeof(KafkaAot.KafkaService);
    Console.WriteLine($"[PASS] KafkaService 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}