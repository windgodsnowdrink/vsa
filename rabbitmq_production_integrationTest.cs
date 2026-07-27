#load "rabbitmq_production_integration.cs"

Console.WriteLine("=== rabbitmq_production_integration Test ===");

try
{
    var t0 = typeof(AdvancedRabbitMQFeatures);
    Console.WriteLine($"[PASS] AdvancedRabbitMQFeatures 存在");
    var t1 = typeof(RabbitMQOptions);
    Console.WriteLine($"[PASS] RabbitMQOptions 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(RabbitMQProducer);
    Console.WriteLine($"[PASS] RabbitMQProducer 存在");
    var t4 = typeof(RabbitMQConsumer);
    Console.WriteLine($"[PASS] RabbitMQConsumer 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}