#load "eventstore_service.cs"

Console.WriteLine("=== eventstore_service Test ===");

try
{
    var t0 = typeof(EventStoreOptions);
    Console.WriteLine($"[PASS] EventStoreOptions 存在");
    var t1 = typeof(EventStoreService);
    Console.WriteLine($"[PASS] EventStoreService 存在");
    var t2 = typeof(EventDataPooledPolicy);
    Console.WriteLine($"[PASS] EventDataPooledPolicy 存在");
    var t3 = typeof(EventStoreDemo);
    Console.WriteLine($"[PASS] EventStoreDemo 存在");
    var t4 = typeof(EventStoreProcessor);
    Console.WriteLine($"[PASS] EventStoreProcessor 存在");
    var t5 = typeof(EventContext);
    Console.WriteLine($"[PASS] EventContext 存在");
    var t6 = typeof(EventContextPooledPolicy);
    Console.WriteLine($"[PASS] EventContextPooledPolicy 存在");
    var t7 = typeof(UserCreatedEvent);
    Console.WriteLine($"[PASS] UserCreatedEvent record 存在");
    var t8 = typeof(EventData);
    Console.WriteLine($"[PASS] EventData record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}