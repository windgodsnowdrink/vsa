#load "csharp_nats_integration.cs"

Console.WriteLine("=== csharp_nats_integration Test ===");

try
{
    var t0 = typeof(NatsOptions);
    Console.WriteLine($"[PASS] NatsOptions 存在");
    var t1 = typeof(NatsPublisher);
    Console.WriteLine($"[PASS] NatsPublisher 存在");
    var t2 = typeof(NatsSubscriber);
    Console.WriteLine($"[PASS] NatsSubscriber 存在");
    var t3 = typeof(NatsServiceCollectionExtensions);
    Console.WriteLine($"[PASS] NatsServiceCollectionExtensions 存在");
    var t4 = typeof(NatsRequestResponse);
    Console.WriteLine($"[PASS] NatsRequestResponse 存在");
    var t5 = typeof(NatsPersistentStorage);
    Console.WriteLine($"[PASS] NatsPersistentStorage 存在");
    var t6 = typeof(NatsTracingExtensions);
    Console.WriteLine($"[PASS] NatsTracingExtensions 存在");
    var t7 = typeof(NatsTracingOptions);
    Console.WriteLine($"[PASS] NatsTracingOptions 存在");
    var t8 = typeof(NatsTracingInterceptor);
    Console.WriteLine($"[PASS] NatsTracingInterceptor 存在");
    var t9 = typeof(NatsMetrics);
    Console.WriteLine($"[PASS] NatsMetrics 存在");
    var t10 = typeof(NatsClusterExtensions);
    Console.WriteLine($"[PASS] NatsClusterExtensions 存在");
    var t11 = typeof(NatsClusterOptions);
    Console.WriteLine($"[PASS] NatsClusterOptions 存在");
    var t12 = typeof(NatsClusterManager);
    Console.WriteLine($"[PASS] NatsClusterManager 存在");
    var t13 = typeof(DemoUsage);
    Console.WriteLine($"[PASS] DemoUsage 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}