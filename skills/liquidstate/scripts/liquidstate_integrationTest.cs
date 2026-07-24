#load "liquidstate_integration.cs"

Console.WriteLine("=== liquidstate_integration Test ===");

try
{
    var t0 = typeof(WorkflowStateMachineOptions);
    Console.WriteLine($"[PASS] WorkflowStateMachineOptions 存在");
    var t1 = typeof(WorkflowStateMachine);
    Console.WriteLine($"[PASS] WorkflowStateMachine 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(RedisOptions);
    Console.WriteLine($"[PASS] RedisOptions 存在");
    var t4 = typeof(RedisStateRepository);
    Console.WriteLine($"[PASS] RedisStateRepository 存在");
    var t5 = typeof(AotStateMachineFactory);
    Console.WriteLine($"[PASS] AotStateMachineFactory 存在");
    var t6 = typeof(MonitoredStateMachine);
    Console.WriteLine($"[PASS] MonitoredStateMachine 存在");
    var t7 = typeof(IWorkflowStateMachine);
    Console.WriteLine($"[PASS] IWorkflowStateMachine 接口存在 (IsInterface: {t7.IsInterface})");
    var t8 = typeof(IStateRepository);
    Console.WriteLine($"[PASS] IStateRepository 接口存在 (IsInterface: {t8.IsInterface})");
    var t9 = typeof(WorkflowState);
    Console.WriteLine($"[PASS] WorkflowState enum 存在 (IsEnum: {t9.IsEnum})");
    var t10 = typeof(WorkflowTrigger);
    Console.WriteLine($"[PASS] WorkflowTrigger enum 存在 (IsEnum: {t10.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}