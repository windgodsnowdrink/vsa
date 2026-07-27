#load "elsa_workflow_integration.cs"

Console.WriteLine("=== elsa_workflow_integration Test ===");

try
{
    var t0 = typeof(AdvancedWorkflowService);
    Console.WriteLine($"[PASS] AdvancedWorkflowService 存在");
    var t1 = typeof(WorkflowExtensions);
    Console.WriteLine($"[PASS] WorkflowExtensions 存在");
    var t2 = typeof(ComplexWorkflow);
    Console.WriteLine($"[PASS] ComplexWorkflow 存在");
    var t3 = typeof(IAdvancedWorkflowService);
    Console.WriteLine($"[PASS] IAdvancedWorkflowService 接口存在 (IsInterface: {t3.IsInterface})");

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}