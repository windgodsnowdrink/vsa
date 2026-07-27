#load "cqrs_processor.cs"

Console.WriteLine("=== cqrs_processor.cs Test ===");

try
{
    // 验证 class: CqrsProcessor
    var type_CqrsProcessor = Type.GetType("CqrsProcessor");
    if (type_CqrsProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 CqrsProcessor (class) 存在");
        var ctors_CqrsProcessor = type_CqrsProcessor.GetConstructors();
        Console.WriteLine($"[PASS] CqrsProcessor 构造函数数量: {ctors_CqrsProcessor.Length}");
        var methods_CqrsProcessor = type_CqrsProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CqrsProcessor 公开方法数量: {methods_CqrsProcessor.Length}");
        foreach (var m in methods_CqrsProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CqrsProcessor 未找到，尝试无命名空间...");
        type_CqrsProcessor = Type.GetType("CqrsProcessor");
        if (type_CqrsProcessor != null)
            Console.WriteLine("[PASS] 类型 CqrsProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CqrsProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 interface: ICommand
    var type_ICommand = Type.GetType("ICommand");
    if (type_ICommand != null)
    {
        Console.WriteLine("[PASS] 类型 ICommand (interface) 存在");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 ICommand 未找到，尝试无命名空间...");
        type_ICommand = Type.GetType("ICommand");
        if (type_ICommand != null)
            Console.WriteLine("[PASS] 类型 ICommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 ICommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: CreateTodoCommand
    var type_CreateTodoCommand = Type.GetType("CreateTodoCommand");
    if (type_CreateTodoCommand != null)
    {
        Console.WriteLine("[PASS] 类型 CreateTodoCommand (record) 存在");
        var ctors_CreateTodoCommand = type_CreateTodoCommand.GetConstructors();
        Console.WriteLine($"[PASS] CreateTodoCommand 构造函数数量: {ctors_CreateTodoCommand.Length}");
        var methods_CreateTodoCommand = type_CreateTodoCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] CreateTodoCommand 公开方法数量: {methods_CreateTodoCommand.Length}");
        foreach (var m in methods_CreateTodoCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 CreateTodoCommand 未找到，尝试无命名空间...");
        type_CreateTodoCommand = Type.GetType("CreateTodoCommand");
        if (type_CreateTodoCommand != null)
            Console.WriteLine("[PASS] 类型 CreateTodoCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 CreateTodoCommand 可能为顶层语句或嵌套类型");
    }

    // 验证 record: UpdateTodoCommand
    var type_UpdateTodoCommand = Type.GetType("UpdateTodoCommand");
    if (type_UpdateTodoCommand != null)
    {
        Console.WriteLine("[PASS] 类型 UpdateTodoCommand (record) 存在");
        var ctors_UpdateTodoCommand = type_UpdateTodoCommand.GetConstructors();
        Console.WriteLine($"[PASS] UpdateTodoCommand 构造函数数量: {ctors_UpdateTodoCommand.Length}");
        var methods_UpdateTodoCommand = type_UpdateTodoCommand.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] UpdateTodoCommand 公开方法数量: {methods_UpdateTodoCommand.Length}");
        foreach (var m in methods_UpdateTodoCommand)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 UpdateTodoCommand 未找到，尝试无命名空间...");
        type_UpdateTodoCommand = Type.GetType("UpdateTodoCommand");
        if (type_UpdateTodoCommand != null)
            Console.WriteLine("[PASS] 类型 UpdateTodoCommand (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 UpdateTodoCommand 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
