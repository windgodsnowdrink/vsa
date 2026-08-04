#load "orleans_statemachine_integration.cs"

Console.WriteLine("=== orleans_statemachine_integration Test ===");

try
{
    var t0 = typeof(WorkflowStateMachineGrain);
    Console.WriteLine($"[PASS] WorkflowStateMachineGrain 存在");
    var t1 = typeof(WorkflowStateMachineState);
    Console.WriteLine($"[PASS] WorkflowStateMachineState 存在");
    var t2 = typeof(ServiceCollectionExtensions);
    Console.WriteLine($"[PASS] ServiceCollectionExtensions 存在");
    var t3 = typeof(WorkflowStateMachineService);
    Console.WriteLine($"[PASS] WorkflowStateMachineService 存在");
    var t4 = typeof(OrleansStateMachineOptions);
    Console.WriteLine($"[PASS] OrleansStateMachineOptions 存在");
    var t5 = typeof(IWorkflowStateMachineGrain);
    Console.WriteLine($"[PASS] IWorkflowStateMachineGrain 接口存在 (IsInterface: {t5.IsInterface})");
    var t6 = typeof(IWorkflowStateMachineService);
    Console.WriteLine($"[PASS] IWorkflowStateMachineService 接口存在 (IsInterface: {t6.IsInterface})");
    var t7 = typeof(WorkflowState);
    Console.WriteLine($"[PASS] WorkflowState enum 存在 (IsEnum: {t7.IsEnum})");
    var t8 = typeof(WorkflowTrigger);
    Console.WriteLine($"[PASS] WorkflowTrigger enum 存在 (IsEnum: {t8.IsEnum})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}