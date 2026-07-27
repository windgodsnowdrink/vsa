#load "mongodb_integration.cs"

Console.WriteLine("=== mongodb_integration Test ===");

try
{
    var t0 = typeof(MongoDBIntegration);
    Console.WriteLine($"[PASS] MongoDBIntegration 存在");
    var t1 = typeof(MongoRepository);
    Console.WriteLine($"[PASS] MongoRepository 存在");
    var t2 = typeof(MongoService);
    Console.WriteLine($"[PASS] MongoService 存在");
    var t3 = typeof(User);
    Console.WriteLine($"[PASS] User 存在");
    var t4 = typeof(IMongoRepository);
    Console.WriteLine($"[PASS] IMongoRepository 接口存在 (IsInterface: {t4.IsInterface})");
    var t5 = typeof(IMongoService);
    Console.WriteLine($"[PASS] IMongoService 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}