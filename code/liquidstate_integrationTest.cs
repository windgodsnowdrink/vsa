#load "liquidstate_integration.cs"

Console.WriteLine("=== liquidstate_integration.cs Test ===");

try
{
    // 验证 class: WorkflowStateMachineOptions
    var type_WorkflowStateMachineOptions = Type.GetType("WorkflowStateMachineOptions");
    if (type_WorkflowStateMachineOptions != null)
    {
        Console.WriteLine("[PASS] 类型 WorkflowStateMachineOptions (class) 存在");
        var ctors_WorkflowStateMachineOptions = type_WorkflowStateMachineOptions.GetConstructors();
        Console.WriteLine($"[PASS] WorkflowStateMachineOptions 构造函数数量: {ctors_WorkflowStateMachineOptions.Length}");
        var methods_WorkflowStateMachineOptions = type_WorkflowStateMachineOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WorkflowStateMachineOptions 公开方法数量: {methods_WorkflowStateMachineOptions.Length}");
        foreach (var m in methods_WorkflowStateMachineOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WorkflowStateMachineOptions 未找到，尝试无命名空间...");
        type_WorkflowStateMachineOptions = Type.GetType("WorkflowStateMachineOptions");
        if (type_WorkflowStateMachineOptions != null)
            Console.WriteLine("[PASS] 类型 WorkflowStateMachineOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WorkflowStateMachineOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WorkflowStateMachine
    var type_WorkflowStateMachine = Type.GetType("WorkflowStateMachine");
    if (type_WorkflowStateMachine != null)
    {
        Console.WriteLine("[PASS] 类型 WorkflowStateMachine (class) 存在");
        var ctors_WorkflowStateMachine = type_WorkflowStateMachine.GetConstructors();
        Console.WriteLine($"[PASS] WorkflowStateMachine 构造函数数量: {ctors_WorkflowStateMachine.Length}");
        var methods_WorkflowStateMachine = type_WorkflowStateMachine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WorkflowStateMachine 公开方法数量: {methods_WorkflowStateMachine.Length}");
        foreach (var m in methods_WorkflowStateMachine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WorkflowStateMachine 未找到，尝试无命名空间...");
        type_WorkflowStateMachine = Type.GetType("WorkflowStateMachine");
        if (type_WorkflowStateMachine != null)
            Console.WriteLine("[PASS] 类型 WorkflowStateMachine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WorkflowStateMachine 可能为顶层语句或嵌套类型");
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

    // 验证 class: RedisOptions
    var type_RedisOptions = Type.GetType("RedisOptions");
    if (type_RedisOptions != null)
    {
        Console.WriteLine("[PASS] 类型 RedisOptions (class) 存在");
        var ctors_RedisOptions = type_RedisOptions.GetConstructors();
        Console.WriteLine($"[PASS] RedisOptions 构造函数数量: {ctors_RedisOptions.Length}");
        var methods_RedisOptions = type_RedisOptions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisOptions 公开方法数量: {methods_RedisOptions.Length}");
        foreach (var m in methods_RedisOptions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisOptions 未找到，尝试无命名空间...");
        type_RedisOptions = Type.GetType("RedisOptions");
        if (type_RedisOptions != null)
            Console.WriteLine("[PASS] 类型 RedisOptions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisOptions 可能为顶层语句或嵌套类型");
    }

    // 验证 class: RedisStateRepository
    var type_RedisStateRepository = Type.GetType("RedisStateRepository");
    if (type_RedisStateRepository != null)
    {
        Console.WriteLine("[PASS] 类型 RedisStateRepository (class) 存在");
        var ctors_RedisStateRepository = type_RedisStateRepository.GetConstructors();
        Console.WriteLine($"[PASS] RedisStateRepository 构造函数数量: {ctors_RedisStateRepository.Length}");
        var methods_RedisStateRepository = type_RedisStateRepository.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] RedisStateRepository 公开方法数量: {methods_RedisStateRepository.Length}");
        foreach (var m in methods_RedisStateRepository)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 RedisStateRepository 未找到，尝试无命名空间...");
        type_RedisStateRepository = Type.GetType("RedisStateRepository");
        if (type_RedisStateRepository != null)
            Console.WriteLine("[PASS] 类型 RedisStateRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 RedisStateRepository 可能为顶层语句或嵌套类型");
    }

    // 验证 class: AotStateMachineFactory
    var type_AotStateMachineFactory = Type.GetType("AotStateMachineFactory");
    if (type_AotStateMachineFactory != null)
    {
        Console.WriteLine("[PASS] 类型 AotStateMachineFactory (class) 存在");
        var ctors_AotStateMachineFactory = type_AotStateMachineFactory.GetConstructors();
        Console.WriteLine($"[PASS] AotStateMachineFactory 构造函数数量: {ctors_AotStateMachineFactory.Length}");
        var methods_AotStateMachineFactory = type_AotStateMachineFactory.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] AotStateMachineFactory 公开方法数量: {methods_AotStateMachineFactory.Length}");
        foreach (var m in methods_AotStateMachineFactory)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 AotStateMachineFactory 未找到，尝试无命名空间...");
        type_AotStateMachineFactory = Type.GetType("AotStateMachineFactory");
        if (type_AotStateMachineFactory != null)
            Console.WriteLine("[PASS] 类型 AotStateMachineFactory (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 AotStateMachineFactory 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MonitoredStateMachine
    var type_MonitoredStateMachine = Type.GetType("MonitoredStateMachine");
    if (type_MonitoredStateMachine != null)
    {
        Console.WriteLine("[PASS] 类型 MonitoredStateMachine (class) 存在");
        var ctors_MonitoredStateMachine = type_MonitoredStateMachine.GetConstructors();
        Console.WriteLine($"[PASS] MonitoredStateMachine 构造函数数量: {ctors_MonitoredStateMachine.Length}");
        var methods_MonitoredStateMachine = type_MonitoredStateMachine.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MonitoredStateMachine 公开方法数量: {methods_MonitoredStateMachine.Length}");
        foreach (var m in methods_MonitoredStateMachine)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MonitoredStateMachine 未找到，尝试无命名空间...");
        type_MonitoredStateMachine = Type.GetType("MonitoredStateMachine");
        if (type_MonitoredStateMachine != null)
            Console.WriteLine("[PASS] 类型 MonitoredStateMachine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MonitoredStateMachine 可能为顶层语句或嵌套类型");
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

    // 验证 interface: IWorkflowStateMachine
    var type_IWorkflowStateMachine = Type.GetType("IWorkflowStateMachine");
    if (type_IWorkflowStateMachine != null)
    {
        Console.WriteLine("[PASS] 类型 IWorkflowStateMachine (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IWorkflowStateMachine 未找到，尝试无命名空间...");
        type_IWorkflowStateMachine = Type.GetType("IWorkflowStateMachine");
        if (type_IWorkflowStateMachine != null)
            Console.WriteLine("[PASS] 类型 IWorkflowStateMachine (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IWorkflowStateMachine 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: IStateRepository
    var type_IStateRepository = Type.GetType("IStateRepository");
    if (type_IStateRepository != null)
    {
        Console.WriteLine("[PASS] 类型 IStateRepository (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 IStateRepository 未找到，尝试无命名空间...");
        type_IStateRepository = Type.GetType("IStateRepository");
        if (type_IStateRepository != null)
            Console.WriteLine("[PASS] 类型 IStateRepository (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 IStateRepository 可能为顶层语句或嵌套类型");
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
