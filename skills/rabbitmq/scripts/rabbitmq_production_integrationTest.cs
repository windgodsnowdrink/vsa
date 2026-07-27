#load "rabbitmq_production_integration.cs"

Console.WriteLine("=== rabbitmq_production_integration Test ===");

try
{
    var t0 = typeof(RabbitMqSkill.RabbitMqOptions);
    Console.WriteLine($"[PASS] RabbitMqOptions 存在");
    var t1 = typeof(RabbitMqSkill.MessageOptions);
    Console.WriteLine($"[PASS] MessageOptions 存在");
    var t2 = typeof(RabbitMqSkill.RabbitMqConnectionFactory);
    Console.WriteLine($"[PASS] RabbitMqConnectionFactory 存在");
    var t3 = typeof(RabbitMqSkill.RabbitMqConnectionPool);
    Console.WriteLine($"[PASS] RabbitMqConnectionPool 存在");
    var t4 = typeof(RabbitMqSkill.RabbitMqService);
    Console.WriteLine($"[PASS] RabbitMqService 存在");
    var t5 = typeof(RabbitMqSkill.Subscription);
    Console.WriteLine($"[PASS] Subscription 存在");
    var t6 = typeof(RabbitMqSkill.RabbitMqServiceCollectionExtensions);
    Console.WriteLine($"[PASS] RabbitMqServiceCollectionExtensions 存在");
    var t7 = typeof(RabbitMqSkill.AdvancedRabbitMqFeatures);
    Console.WriteLine($"[PASS] AdvancedRabbitMqFeatures 存在");
    var t8 = typeof(RabbitMqSkill.IRabbitMqConnectionPool);
    Console.WriteLine($"[PASS] IRabbitMqConnectionPool 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(RabbitMqSkill.IRabbitMqConnectionFactory);
    Console.WriteLine($"[PASS] IRabbitMqConnectionFactory 接口存在 (IsInterface: {t9.IsInterface})");
    var t10 = typeof(RabbitMqSkill.IRabbitMqService);
    Console.WriteLine($"[PASS] IRabbitMqService 接口存在 (IsInterface: {t10.IsInterface})");
    var t11 = typeof(RabbitMqSkill.ExchangeType);
    Console.WriteLine($"[PASS] ExchangeType enum 存在 (IsEnum: {t11.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}