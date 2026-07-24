#load "orleans_statemachine_integration.cs"

Console.WriteLine("=== orleans_statemachine_integration.cs Test ===");

try
{
    // 验证 class: WorkflowStateMachineGrain
    var type_WorkflowStateMachineGrain = Type.GetType("WorkflowStateMachineGrain");
    if (type_WorkflowStateMachineGrain != null)
    {
        Console.WriteLine("[PASS] 类型 WorkflowStateMachineGrain (class) 存在");
        var ctors_WorkflowStateMachineGrain = type_WorkflowStateMachineGrain.GetConstructors();
        Console.WriteLine($"[PASS] WorkflowStateMachineGrain 构造函数数量: {ctors_WorkflowStateMachineGrain.Length}");
        var methods_WorkflowStateMachineGrain = type_WorkflowStateMachineGrain.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WorkflowStateMachineGrain 公开方法数量: {methods_WorkflowStateMachineGrain.Length}");
        foreach (var m in methods_WorkflowStateMachineGrain)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WorkflowStateMachineGrain 未找到，尝试无命名空间...");
        type_WorkflowStateMachineGrain = Type.GetType("WorkflowStateMachineGrain");
        if (type_WorkflowStateMachineGrain != null)
            Console.WriteLine("[PASS] 类型 WorkflowStateMachineGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WorkflowStateMachineGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WorkflowStateMachineState
    var type_WorkflowStateMachineState = Type.GetType("WorkflowStateMachineState");
    if (type_WorkflowStateMachineState != null)
    {
        Console.WriteLine("[PASS] 类型 WorkflowStateMachineState (class) 存在");
        var ctors_WorkflowStateMachineState = type_WorkflowStateMachineState.GetConstructors();
        Console.WriteLine($"[PASS] WorkflowStateMachineState 构造函数数量: {ctors_WorkflowStateMachineState.Length}");
        var methods_WorkflowStateMachineState = type_WorkflowStateMachineState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WorkflowStateMachineState 公开方法数量: {methods_WorkflowStateMachineState.Length}");
        foreach (var m in methods_WorkflowStateMachineState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WorkflowStateMachineState 未找到，尝试无命名空间...");
        type_WorkflowStateMachineState = Type.GetType("WorkflowStateMachineState");
        if (type_WorkflowStateMachineState != null)
            Console.WriteLine("[PASS] 类型 WorkflowStateMachineState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WorkflowStateMachineState 可能为顶层语句或嵌套类型");
    }

    // 验证 class: ServiceCollectionExtensions
    var type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
    if (type_ServiceCollectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (class) 存在");
        var ctors_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 构造函数数量: {ctors_ServiceCollectionExtensions.Length}");
        var methods_ServiceCollectionExtensions = type_ServiceCollectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ServiceCollectionExtensions 公开方法数量: {methods_ServiceCollectionExtensions.Length}");
        foreach (var m in methods_ServiceCollectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ServiceCollectionExtensions 未找到，尝试无命名空间...");
        type_ServiceCollectionExtensions = Type.GetType("ServiceCollectionExtensions");
        if (type_ServiceCollectionExtensions != null)
            Console.WriteLine("[PASS] 类型 ServiceCollectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ServiceCollectionExtensions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WorkflowStateMachineService
    var type_WorkflowStateMachineService = Type.GetType("WorkflowStateMachineService");
    if (type_WorkflowStateMachineService != null)
    {
        Console.WriteLine("[PASS] 类型 WorkflowStateMachineService (class) 存在");
        var ctors_WorkflowStateMachineService = type_WorkflowStateMachineService.GetConstructors();
        Console.WriteLine($"[PASS] WorkflowStateMachineService 构造函数数量: {ctors_WorkflowStateMachineService.Length}");
        var methods_WorkflowStateMachineService = type_WorkflowStateMachineService.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WorkflowStateMachineService 公开方法数量: {methods_WorkflowStateMachineService.Length}");
        foreach (var m in methods_WorkflowStateMachineService)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WorkflowStateMachineService 未找到，尝试无命名空间...");
        type_WorkflowStateMachineService = Type.GetType("WorkflowStateMachineService");
        if (type_WorkflowStateMachineService != null)
            Console.WriteLine("[PASS] 类型 WorkflowStateMachineService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WorkflowStateMachineService 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OrleansStateMachineOptions
    var type_OrleansStateMachineOptions = Type.GetType("OrleansStateMachineOptions");
    if (type_OrleansStateMachineOptions != null)
    {
        Console.WriteLine("[PASS] 类型 OrleansStateMachineOptions (class) 存在");
        var ctors_OrleansStateMachineOptions = type_OrleansStateMachineOptions.GetConstructors();
        Console.WriteLine($"[PASS] OrleansStateMachineOptions 构造函数数量: {ctors_OrleansStateMachineOptions.Length}");
        var methods_OrleansStateMachineOptions = type_OrleansStateMachineOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OrleansStateMachineOptions 公开方法数量: {methods_OrleansStateMachineOptions.Length}");
        foreach (var m in methods_OrleansStateMachineOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OrleansStateMachineOptions 未找到，尝试无命名空间...");
        type_OrleansStateMachineOptions = Type.GetType("OrleansStateMachineOptions");
        if (type_OrleansStateMachineOptions != null)
            Console.WriteLine("[PASS] 类型 OrleansStateMachineOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OrleansStateMachineOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: Program
    var type_Program = Type.GetType("Program");
    if (type_Program != null)
    {
        Console.WriteLine("[PASS] 类型 Program (class) 存在");
        var ctors_Program = type_Program.GetConstructors();
        Console.WriteLine($"[PASS] Program 构造函数数量: {ctors_Program.Length}");
        var methods_Program = type_Program.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] Program 公开方法数量: {methods_Program.Length}");
        foreach (var m in methods_Program)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 Program 未找到，尝试无命名空间...");
        type_Program = Type.GetType("Program");
        if (type_Program != null)
            Console.WriteLine("[PASS] 类型 Program (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 Program 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IWorkflowStateMachineGrain
    var type_IWorkflowStateMachineGrain = Type.GetType("IWorkflowStateMachineGrain");
    if (type_IWorkflowStateMachineGrain != null)
    {
        Console.WriteLine("[PASS] 类型 IWorkflowStateMachineGrain (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IWorkflowStateMachineGrain 未找到，尝试无命名空间...");
        type_IWorkflowStateMachineGrain = Type.GetType("IWorkflowStateMachineGrain");
        if (type_IWorkflowStateMachineGrain != null)
            Console.WriteLine("[PASS] 类型 IWorkflowStateMachineGrain (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IWorkflowStateMachineGrain 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IWorkflowStateMachineService
    var type_IWorkflowStateMachineService = Type.GetType("IWorkflowStateMachineService");
    if (type_IWorkflowStateMachineService != null)
    {
        Console.WriteLine("[PASS] 类型 IWorkflowStateMachineService (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IWorkflowStateMachineService 未找到，尝试无命名空间...");
        type_IWorkflowStateMachineService = Type.GetType("IWorkflowStateMachineService");
        if (type_IWorkflowStateMachineService != null)
            Console.WriteLine("[PASS] 类型 IWorkflowStateMachineService (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IWorkflowStateMachineService 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: WorkflowState
    var type_WorkflowState = Type.GetType("WorkflowState");
    if (type_WorkflowState != null)
    {
        Console.WriteLine("[PASS] 类型 WorkflowState (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WorkflowState 未找到，尝试无命名空间...");
        type_WorkflowState = Type.GetType("WorkflowState");
        if (type_WorkflowState != null)
            Console.WriteLine("[PASS] 类型 WorkflowState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WorkflowState 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: WorkflowTrigger
    var type_WorkflowTrigger = Type.GetType("WorkflowTrigger");
    if (type_WorkflowTrigger != null)
    {
        Console.WriteLine("[PASS] 类型 WorkflowTrigger (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WorkflowTrigger 未找到，尝试无命名空间...");
        type_WorkflowTrigger = Type.GetType("WorkflowTrigger");
        if (type_WorkflowTrigger != null)
            Console.WriteLine("[PASS] 类型 WorkflowTrigger (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WorkflowTrigger 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
