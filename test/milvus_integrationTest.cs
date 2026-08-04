#load "milvus_integration.cs"

Console.WriteLine("=== milvus_integration Test ===");

try
{
    var t0 = typeof(MilvusIntegration.MilvusOptions);
    Console.WriteLine($"[PASS] MilvusOptions 存在");
    var t1 = typeof(MilvusIntegration.MilvusService);
    Console.WriteLine($"[PASS] MilvusService 存在");
    var t2 = typeof(MilvusIntegration.ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(MilvusIntegration.ApplicationBuilderExtensions);
    Console.WriteLine($"[PASS] ApplicationBuilderExtensions 存在");
    var t4 = typeof(MilvusIntegration.ExampleUsage);
    Console.WriteLine($"[PASS] ExampleUsage 存在");
    var t5 = typeof(MilvusIntegration.IMilvusService);
    Console.WriteLine($"[PASS] IMilvusService 接口存在 (IsInterface: {t5.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}