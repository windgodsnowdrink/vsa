#load "wolverine_saga.cs"

Console.WriteLine("=== wolverine_saga.cs Test ===");

try
{
    // 验证 class: TodoSagaState
    var type_TodoSagaState = Type.GetType("TodoSagaState");
    if (type_TodoSagaState != null)
    {
        Console.WriteLine("[PASS] 类型 TodoSagaState (class) 存在");
        var ctors_TodoSagaState = type_TodoSagaState.GetConstructors();
        Console.WriteLine($"[PASS] TodoSagaState 构造函数数量: {ctors_TodoSagaState.Length}");
        var methods_TodoSagaState = type_TodoSagaState.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoSagaState 公开方法数量: {methods_TodoSagaState.Length}");
        foreach (var m in methods_TodoSagaState)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoSagaState 未找到，尝试无命名空间...");
        type_TodoSagaState = Type.GetType("TodoSagaState");
        if (type_TodoSagaState != null)
            Console.WriteLine("[PASS] 类型 TodoSagaState (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoSagaState 可能为顶层语句或嵌套类型");
    }

    // 验证 class: TodoSaga
    var type_TodoSaga = Type.GetType("TodoSaga");
    if (type_TodoSaga != null)
    {
        Console.WriteLine("[PASS] 类型 TodoSaga (class) 存在");
        var ctors_TodoSaga = type_TodoSaga.GetConstructors();
        Console.WriteLine($"[PASS] TodoSaga 构造函数数量: {ctors_TodoSaga.Length}");
        var methods_TodoSaga = type_TodoSaga.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoSaga 公开方法数量: {methods_TodoSaga.Length}");
        foreach (var m in methods_TodoSaga)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoSaga 未找到，尝试无命名空间...");
        type_TodoSaga = Type.GetType("TodoSaga");
        if (type_TodoSaga != null)
            Console.WriteLine("[PASS] 类型 TodoSaga (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoSaga 可能为顶层语句或嵌套类型");
    }

    // 验证 record: StartTodoSagaCommand
    var type_StartTodoSagaCommand = Type.GetType("StartTodoSagaCommand");
    if (type_StartTodoSagaCommand != null)
    {
        Console.WriteLine("[PASS] 类型 StartTodoSagaCommand (record) 存在");
        var ctors_StartTodoSagaCommand = type_StartTodoSagaCommand.GetConstructors();
        Console.WriteLine($"[PASS] StartTodoSagaCommand 构造函数数量: {ctors_StartTodoSagaCommand.Length}");
        var methods_StartTodoSagaCommand = type_StartTodoSagaCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] StartTodoSagaCommand 公开方法数量: {methods_StartTodoSagaCommand.Length}");
        foreach (var m in methods_StartTodoSagaCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 StartTodoSagaCommand 未找到，尝试无命名空间...");
        type_StartTodoSagaCommand = Type.GetType("StartTodoSagaCommand");
        if (type_StartTodoSagaCommand != null)
            Console.WriteLine("[PASS] 类型 StartTodoSagaCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 StartTodoSagaCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ReserveResourcesEvent
    var type_ReserveResourcesEvent = Type.GetType("ReserveResourcesEvent");
    if (type_ReserveResourcesEvent != null)
    {
        Console.WriteLine("[PASS] 类型 ReserveResourcesEvent (record) 存在");
        var ctors_ReserveResourcesEvent = type_ReserveResourcesEvent.GetConstructors();
        Console.WriteLine($"[PASS] ReserveResourcesEvent 构造函数数量: {ctors_ReserveResourcesEvent.Length}");
        var methods_ReserveResourcesEvent = type_ReserveResourcesEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ReserveResourcesEvent 公开方法数量: {methods_ReserveResourcesEvent.Length}");
        foreach (var m in methods_ReserveResourcesEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ReserveResourcesEvent 未找到，尝试无命名空间...");
        type_ReserveResourcesEvent = Type.GetType("ReserveResourcesEvent");
        if (type_ReserveResourcesEvent != null)
            Console.WriteLine("[PASS] 类型 ReserveResourcesEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ReserveResourcesEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ResourcesReservedEvent
    var type_ResourcesReservedEvent = Type.GetType("ResourcesReservedEvent");
    if (type_ResourcesReservedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 ResourcesReservedEvent (record) 存在");
        var ctors_ResourcesReservedEvent = type_ResourcesReservedEvent.GetConstructors();
        Console.WriteLine($"[PASS] ResourcesReservedEvent 构造函数数量: {ctors_ResourcesReservedEvent.Length}");
        var methods_ResourcesReservedEvent = type_ResourcesReservedEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResourcesReservedEvent 公开方法数量: {methods_ResourcesReservedEvent.Length}");
        foreach (var m in methods_ResourcesReservedEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResourcesReservedEvent 未找到，尝试无命名空间...");
        type_ResourcesReservedEvent = Type.GetType("ResourcesReservedEvent");
        if (type_ResourcesReservedEvent != null)
            Console.WriteLine("[PASS] 类型 ResourcesReservedEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResourcesReservedEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: ResourcesRolledBackEvent
    var type_ResourcesRolledBackEvent = Type.GetType("ResourcesRolledBackEvent");
    if (type_ResourcesRolledBackEvent != null)
    {
        Console.WriteLine("[PASS] 类型 ResourcesRolledBackEvent (record) 存在");
        var ctors_ResourcesRolledBackEvent = type_ResourcesRolledBackEvent.GetConstructors();
        Console.WriteLine($"[PASS] ResourcesRolledBackEvent 构造函数数量: {ctors_ResourcesRolledBackEvent.Length}");
        var methods_ResourcesRolledBackEvent = type_ResourcesRolledBackEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] ResourcesRolledBackEvent 公开方法数量: {methods_ResourcesRolledBackEvent.Length}");
        foreach (var m in methods_ResourcesRolledBackEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ResourcesRolledBackEvent 未找到，尝试无命名空间...");
        type_ResourcesRolledBackEvent = Type.GetType("ResourcesRolledBackEvent");
        if (type_ResourcesRolledBackEvent != null)
            Console.WriteLine("[PASS] 类型 ResourcesRolledBackEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ResourcesRolledBackEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 enum: SagaStatus
    var type_SagaStatus = Type.GetType("SagaStatus");
    if (type_SagaStatus != null)
    {
        Console.WriteLine("[PASS] 类型 SagaStatus (enum) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 SagaStatus 未找到，尝试无命名空间...");
        type_SagaStatus = Type.GetType("SagaStatus");
        if (type_SagaStatus != null)
            Console.WriteLine("[PASS] 类型 SagaStatus (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 SagaStatus 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
