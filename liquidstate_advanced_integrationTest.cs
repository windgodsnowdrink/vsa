#load "liquidstate_advanced_integration.cs"

Console.WriteLine("=== liquidstate_advanced_integration Test ===");

try
{
    var t0 = typeof(RedisStateRepository);
    Console.WriteLine($"[PASS] RedisStateRepository 存在");
    var t1 = typeof(AotStateMachineFactory);
    Console.WriteLine($"[PASS] AotStateMachineFactory 存在");
    var t2 = typeof(MonitoredStateMachine);
    Console.WriteLine($"[PASS] MonitoredStateMachine 存在");
    var t3 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t4 = typeof(IStateRepository);
    Console.WriteLine($"[PASS] IStateRepository 接口存在 (IsInterface: {t4.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}