#load "state_machine_flow.cs"

Console.WriteLine("=== state_machine_flow.cs Test ===");

try
{
    // 验证 class: StateMachineFlow
    var type_StateMachineFlow = Type.GetType("StateMachineFlow");
    if (type_StateMachineFlow != null)
    {
        Console.WriteLine("[PASS] 类型 StateMachineFlow (class) 存在");
        var ctors_StateMachineFlow = type_StateMachineFlow.GetConstructors();
        Console.WriteLine($"[PASS] StateMachineFlow 构造函数数量: {ctors_StateMachineFlow.Length}");
        var methods_StateMachineFlow = type_StateMachineFlow.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StateMachineFlow 公开方法数量: {methods_StateMachineFlow.Length}");
        foreach (var m in methods_StateMachineFlow)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StateMachineFlow 未找到，尝试无命名空间...");
        type_StateMachineFlow = Type.GetType("StateMachineFlow");
        if (type_StateMachineFlow != null)
            Console.WriteLine("[PASS] 类型 StateMachineFlow (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StateMachineFlow 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TaskItem
    var type_TaskItem = Type.GetType("TaskItem");
    if (type_TaskItem != null)
    {
        Console.WriteLine("[PASS] 类型 TaskItem (record) 存在");
        var ctors_TaskItem = type_TaskItem.GetConstructors();
        Console.WriteLine($"[PASS] TaskItem 构造函数数量: {ctors_TaskItem.Length}");
        var methods_TaskItem = type_TaskItem.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TaskItem 公开方法数量: {methods_TaskItem.Length}");
        foreach (var m in methods_TaskItem)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TaskItem 未找到，尝试无命名空间...");
        type_TaskItem = Type.GetType("TaskItem");
        if (type_TaskItem != null)
            Console.WriteLine("[PASS] 类型 TaskItem (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TaskItem 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TaskState
    var type_TaskState = Type.GetType("TaskState");
    if (type_TaskState != null)
    {
        Console.WriteLine("[PASS] 类型 TaskState (record) 存在");
        var ctors_TaskState = type_TaskState.GetConstructors();
        Console.WriteLine($"[PASS] TaskState 构造函数数量: {ctors_TaskState.Length}");
        var methods_TaskState = type_TaskState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TaskState 公开方法数量: {methods_TaskState.Length}");
        foreach (var m in methods_TaskState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TaskState 未找到，尝试无命名空间...");
        type_TaskState = Type.GetType("TaskState");
        if (type_TaskState != null)
            Console.WriteLine("[PASS] 类型 TaskState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TaskState 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: State
    var type_State = Type.GetType("State");
    if (type_State != null)
    {
        Console.WriteLine("[PASS] 类型 State (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 State 未找到，尝试无命名空间...");
        type_State = Type.GetType("State");
        if (type_State != null)
            Console.WriteLine("[PASS] 类型 State (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 State 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
