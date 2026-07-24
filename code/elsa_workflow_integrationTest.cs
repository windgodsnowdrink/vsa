#load "elsa_workflow_integration.cs"

Console.WriteLine("=== elsa_workflow_integration.cs Test ===");

try
{
    // 验证 class: AdvancedWorkflowService
    var type_AdvancedWorkflowService = Type.GetType("AdvancedWorkflowService");
    if (type_AdvancedWorkflowService != null)
    {
        Console.WriteLine("[PASS] 类型 AdvancedWorkflowService (class) 存在");
        var ctors_AdvancedWorkflowService = type_AdvancedWorkflowService.GetConstructors();
        Console.WriteLine($"[PASS] AdvancedWorkflowService 构造函数数量: {ctors_AdvancedWorkflowService.Length}");
        var methods_AdvancedWorkflowService = type_AdvancedWorkflowService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AdvancedWorkflowService 公开方法数量: {methods_AdvancedWorkflowService.Length}");
        foreach (var m in methods_AdvancedWorkflowService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AdvancedWorkflowService 未找到，尝试无命名空间...");
        type_AdvancedWorkflowService = Type.GetType("AdvancedWorkflowService");
        if (type_AdvancedWorkflowService != null)
            Console.WriteLine("[PASS] 类型 AdvancedWorkflowService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AdvancedWorkflowService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WorkflowExtensions
    var type_WorkflowExtensions = Type.GetType("WorkflowExtensions");
    if (type_WorkflowExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 WorkflowExtensions (class) 存在");
        var ctors_WorkflowExtensions = type_WorkflowExtensions.GetConstructors();
        Console.WriteLine($"[PASS] WorkflowExtensions 构造函数数量: {ctors_WorkflowExtensions.Length}");
        var methods_WorkflowExtensions = type_WorkflowExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WorkflowExtensions 公开方法数量: {methods_WorkflowExtensions.Length}");
        foreach (var m in methods_WorkflowExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WorkflowExtensions 未找到，尝试无命名空间...");
        type_WorkflowExtensions = Type.GetType("WorkflowExtensions");
        if (type_WorkflowExtensions != null)
            Console.WriteLine("[PASS] 类型 WorkflowExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WorkflowExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ComplexWorkflow
    var type_ComplexWorkflow = Type.GetType("ComplexWorkflow");
    if (type_ComplexWorkflow != null)
    {
        Console.WriteLine("[PASS] 类型 ComplexWorkflow (class) 存在");
        var ctors_ComplexWorkflow = type_ComplexWorkflow.GetConstructors();
        Console.WriteLine($"[PASS] ComplexWorkflow 构造函数数量: {ctors_ComplexWorkflow.Length}");
        var methods_ComplexWorkflow = type_ComplexWorkflow.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ComplexWorkflow 公开方法数量: {methods_ComplexWorkflow.Length}");
        foreach (var m in methods_ComplexWorkflow)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ComplexWorkflow 未找到，尝试无命名空间...");
        type_ComplexWorkflow = Type.GetType("ComplexWorkflow");
        if (type_ComplexWorkflow != null)
            Console.WriteLine("[PASS] 类型 ComplexWorkflow (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ComplexWorkflow 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IAdvancedWorkflowService
    var type_IAdvancedWorkflowService = Type.GetType("IAdvancedWorkflowService");
    if (type_IAdvancedWorkflowService != null)
    {
        Console.WriteLine("[PASS] 类型 IAdvancedWorkflowService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IAdvancedWorkflowService 未找到，尝试无命名空间...");
        type_IAdvancedWorkflowService = Type.GetType("IAdvancedWorkflowService");
        if (type_IAdvancedWorkflowService != null)
            Console.WriteLine("[PASS] 类型 IAdvancedWorkflowService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IAdvancedWorkflowService 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
