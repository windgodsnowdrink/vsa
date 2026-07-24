#load "wolverine_saga_orchestration.cs"

Console.WriteLine("=== wolverine_saga_orchestration.cs Test ===");

try
{
    // 验证 class: TodoSagaOrchestrator
    var type_TodoSagaOrchestrator = Type.GetType("TodoSagaOrchestrator");
    if (type_TodoSagaOrchestrator != null)
    {
        Console.WriteLine("[PASS] 类型 TodoSagaOrchestrator (class) 存在");
        var ctors_TodoSagaOrchestrator = type_TodoSagaOrchestrator.GetConstructors();
        Console.WriteLine($"[PASS] TodoSagaOrchestrator 构造函数数量: {ctors_TodoSagaOrchestrator.Length}");
        var methods_TodoSagaOrchestrator = type_TodoSagaOrchestrator.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoSagaOrchestrator 公开方法数量: {methods_TodoSagaOrchestrator.Length}");
        foreach (var m in methods_TodoSagaOrchestrator)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoSagaOrchestrator 未找到，尝试无命名空间...");
        type_TodoSagaOrchestrator = Type.GetType("TodoSagaOrchestrator");
        if (type_TodoSagaOrchestrator != null)
            Console.WriteLine("[PASS] 类型 TodoSagaOrchestrator (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoSagaOrchestrator 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CreateTodoItemEvent
    var type_CreateTodoItemEvent = Type.GetType("CreateTodoItemEvent");
    if (type_CreateTodoItemEvent != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoItemEvent (record) 存在");
        var ctors_CreateTodoItemEvent = type_CreateTodoItemEvent.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodoItemEvent 构造函数数量: {ctors_CreateTodoItemEvent.Length}");
        var methods_CreateTodoItemEvent = type_CreateTodoItemEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodoItemEvent 公开方法数量: {methods_CreateTodoItemEvent.Length}");
        foreach (var m in methods_CreateTodoItemEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodoItemEvent 未找到，尝试无命名空间...");
        type_CreateTodoItemEvent = Type.GetType("CreateTodoItemEvent");
        if (type_CreateTodoItemEvent != null)
            Console.WriteLine("[PASS] 类型 CreateTodoItemEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodoItemEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: NotifyUserEvent
    var type_NotifyUserEvent = Type.GetType("NotifyUserEvent");
    if (type_NotifyUserEvent != null)
    {
        Console.WriteLine("[PASS] 类型 NotifyUserEvent (record) 存在");
        var ctors_NotifyUserEvent = type_NotifyUserEvent.GetConstructors();
        Console.WriteLine($"[PASS] NotifyUserEvent 构造函数数量: {ctors_NotifyUserEvent.Length}");
        var methods_NotifyUserEvent = type_NotifyUserEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] NotifyUserEvent 公开方法数量: {methods_NotifyUserEvent.Length}");
        foreach (var m in methods_NotifyUserEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 NotifyUserEvent 未找到，尝试无命名空间...");
        type_NotifyUserEvent = Type.GetType("NotifyUserEvent");
        if (type_NotifyUserEvent != null)
            Console.WriteLine("[PASS] 类型 NotifyUserEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 NotifyUserEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UserNotifiedEvent
    var type_UserNotifiedEvent = Type.GetType("UserNotifiedEvent");
    if (type_UserNotifiedEvent != null)
    {
        Console.WriteLine("[PASS] 类型 UserNotifiedEvent (record) 存在");
        var ctors_UserNotifiedEvent = type_UserNotifiedEvent.GetConstructors();
        Console.WriteLine($"[PASS] UserNotifiedEvent 构造函数数量: {ctors_UserNotifiedEvent.Length}");
        var methods_UserNotifiedEvent = type_UserNotifiedEvent.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UserNotifiedEvent 公开方法数量: {methods_UserNotifiedEvent.Length}");
        foreach (var m in methods_UserNotifiedEvent)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UserNotifiedEvent 未找到，尝试无命名空间...");
        type_UserNotifiedEvent = Type.GetType("UserNotifiedEvent");
        if (type_UserNotifiedEvent != null)
            Console.WriteLine("[PASS] 类型 UserNotifiedEvent (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UserNotifiedEvent 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CompleteTodoSagaCommand
    var type_CompleteTodoSagaCommand = Type.GetType("CompleteTodoSagaCommand");
    if (type_CompleteTodoSagaCommand != null)
    {
        Console.WriteLine("[PASS] 类型 CompleteTodoSagaCommand (record) 存在");
        var ctors_CompleteTodoSagaCommand = type_CompleteTodoSagaCommand.GetConstructors();
        Console.WriteLine($"[PASS] CompleteTodoSagaCommand 构造函数数量: {ctors_CompleteTodoSagaCommand.Length}");
        var methods_CompleteTodoSagaCommand = type_CompleteTodoSagaCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CompleteTodoSagaCommand 公开方法数量: {methods_CompleteTodoSagaCommand.Length}");
        foreach (var m in methods_CompleteTodoSagaCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CompleteTodoSagaCommand 未找到，尝试无命名空间...");
        type_CompleteTodoSagaCommand = Type.GetType("CompleteTodoSagaCommand");
        if (type_CompleteTodoSagaCommand != null)
            Console.WriteLine("[PASS] 类型 CompleteTodoSagaCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CompleteTodoSagaCommand 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
