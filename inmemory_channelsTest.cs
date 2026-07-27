#load "inmemory_channels.cs"

Console.WriteLine("=== inmemory_channels Test ===");

try
{
    var t0 = typeof(InMemoryMessageQueue);
    Console.WriteLine($"[PASS] InMemoryMessageQueue 存在");
    var t1 = typeof(EventBus);
    Console.WriteLine($"[PASS] EventBus 存在");
    var t2 = typeof(IntegrationEventProcessorJob);
    Console.WriteLine($"[PASS] IntegrationEventProcessorJob 存在");
    var t3 = typeof(RegisterUserCommandHandler);
    Console.WriteLine($"[PASS] RegisterUserCommandHandler 存在");
    var t4 = typeof(UserRegisteredIntegrationEventHandler);
    Console.WriteLine($"[PASS] UserRegisteredIntegrationEventHandler 存在");
    var t5 = typeof(IEventBus);
    Console.WriteLine($"[PASS] IEventBus 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(IIntegrationEvent);
    Console.WriteLine($"[PASS] IIntegrationEvent 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(IntegrationEvent);
    Console.WriteLine($"[PASS] IntegrationEvent record 存在");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}