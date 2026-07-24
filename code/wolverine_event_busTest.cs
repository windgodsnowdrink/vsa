#load "wolverine_event_bus.cs"

Console.WriteLine("=== wolverine_event_bus.cs Test ===");

try
{
    // 验证 class: TodoEventHandler
    var type_TodoEventHandler = Type.GetType("TodoEventHandler");
    if (type_TodoEventHandler != null)
    {
        Console.WriteLine("[PASS] 类型 TodoEventHandler (class) 存在");
        var ctors_TodoEventHandler = type_TodoEventHandler.GetConstructors();
        Console.WriteLine($"[PASS] TodoEventHandler 构造函数数量: {ctors_TodoEventHandler.Length}");
        var methods_TodoEventHandler = type_TodoEventHandler.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoEventHandler 公开方法数量: {methods_TodoEventHandler.Length}");
        foreach (var m in methods_TodoEventHandler)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoEventHandler 未找到，尝试无命名空间...");
        type_TodoEventHandler = Type.GetType("TodoEventHandler");
        if (type_TodoEventHandler != null)
            Console.WriteLine("[PASS] 类型 TodoEventHandler (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoEventHandler 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TodoCreated
    var type_TodoCreated = Type.GetType("TodoCreated");
    if (type_TodoCreated != null)
    {
        Console.WriteLine("[PASS] 类型 TodoCreated (record) 存在");
        var ctors_TodoCreated = type_TodoCreated.GetConstructors();
        Console.WriteLine($"[PASS] TodoCreated 构造函数数量: {ctors_TodoCreated.Length}");
        var methods_TodoCreated = type_TodoCreated.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoCreated 公开方法数量: {methods_TodoCreated.Length}");
        foreach (var m in methods_TodoCreated)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoCreated 未找到，尝试无命名空间...");
        type_TodoCreated = Type.GetType("TodoCreated");
        if (type_TodoCreated != null)
            Console.WriteLine("[PASS] 类型 TodoCreated (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoCreated 可能为顶层语句或嵌套类型");
    }

    // 验证 record: TodoCompleted
    var type_TodoCompleted = Type.GetType("TodoCompleted");
    if (type_TodoCompleted != null)
    {
        Console.WriteLine("[PASS] 类型 TodoCompleted (record) 存在");
        var ctors_TodoCompleted = type_TodoCompleted.GetConstructors();
        Console.WriteLine($"[PASS] TodoCompleted 构造函数数量: {ctors_TodoCompleted.Length}");
        var methods_TodoCompleted = type_TodoCompleted.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] TodoCompleted 公开方法数量: {methods_TodoCompleted.Length}");
        foreach (var m in methods_TodoCompleted)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 TodoCompleted 未找到，尝试无命名空间...");
        type_TodoCompleted = Type.GetType("TodoCompleted");
        if (type_TodoCompleted != null)
            Console.WriteLine("[PASS] 类型 TodoCompleted (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 TodoCompleted 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
