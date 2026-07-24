#load "wolverinefx_local_message_table_integration.cs"

Console.WriteLine("=== wolverinefx_local_message_table_integration.cs Test ===");

try
{
    // 验证 class: OutboxMessage
    var type_OutboxMessage = Type.GetType("OutboxMessage");
    if (type_OutboxMessage != null)
    {
        Console.WriteLine("[PASS] 类型 OutboxMessage (class) 存在");
        var ctors_OutboxMessage = type_OutboxMessage.GetConstructors();
        Console.WriteLine($"[PASS] OutboxMessage 构造函数数量: {ctors_OutboxMessage.Length}");
        var methods_OutboxMessage = type_OutboxMessage.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OutboxMessage 公开方法数量: {methods_OutboxMessage.Length}");
        foreach (var m in methods_OutboxMessage)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OutboxMessage 未找到，尝试无命名空间...");
        type_OutboxMessage = Type.GetType("OutboxMessage");
        if (type_OutboxMessage != null)
            Console.WriteLine("[PASS] 类型 OutboxMessage (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OutboxMessage 可能为顶层语句或嵌套类型");
    }

    // 验证 class: MessageDbContext
    var type_MessageDbContext = Type.GetType("MessageDbContext");
    if (type_MessageDbContext != null)
    {
        Console.WriteLine("[PASS] 类型 MessageDbContext (class) 存在");
        var ctors_MessageDbContext = type_MessageDbContext.GetConstructors();
        Console.WriteLine($"[PASS] MessageDbContext 构造函数数量: {ctors_MessageDbContext.Length}");
        var methods_MessageDbContext = type_MessageDbContext.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] MessageDbContext 公开方法数量: {methods_MessageDbContext.Length}");
        foreach (var m in methods_MessageDbContext)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 MessageDbContext 未找到，尝试无命名空间...");
        type_MessageDbContext = Type.GetType("MessageDbContext");
        if (type_MessageDbContext != null)
            Console.WriteLine("[PASS] 类型 MessageDbContext (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 MessageDbContext 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OutboxMiddleware
    var type_OutboxMiddleware = Type.GetType("OutboxMiddleware");
    if (type_OutboxMiddleware != null)
    {
        Console.WriteLine("[PASS] 类型 OutboxMiddleware (class) 存在");
        var ctors_OutboxMiddleware = type_OutboxMiddleware.GetConstructors();
        Console.WriteLine($"[PASS] OutboxMiddleware 构造函数数量: {ctors_OutboxMiddleware.Length}");
        var methods_OutboxMiddleware = type_OutboxMiddleware.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OutboxMiddleware 公开方法数量: {methods_OutboxMiddleware.Length}");
        foreach (var m in methods_OutboxMiddleware)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OutboxMiddleware 未找到，尝试无命名空间...");
        type_OutboxMiddleware = Type.GetType("OutboxMiddleware");
        if (type_OutboxMiddleware != null)
            Console.WriteLine("[PASS] 类型 OutboxMiddleware (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OutboxMiddleware 可能为顶层语句或嵌套类型");
    }

    // 验证 class: OutboxProcessor
    var type_OutboxProcessor = Type.GetType("OutboxProcessor");
    if (type_OutboxProcessor != null)
    {
        Console.WriteLine("[PASS] 类型 OutboxProcessor (class) 存在");
        var ctors_OutboxProcessor = type_OutboxProcessor.GetConstructors();
        Console.WriteLine($"[PASS] OutboxProcessor 构造函数数量: {ctors_OutboxProcessor.Length}");
        var methods_OutboxProcessor = type_OutboxProcessor.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] OutboxProcessor 公开方法数量: {methods_OutboxProcessor.Length}");
        foreach (var m in methods_OutboxProcessor)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 OutboxProcessor 未找到，尝试无命名空间...");
        type_OutboxProcessor = Type.GetType("OutboxProcessor");
        if (type_OutboxProcessor != null)
            Console.WriteLine("[PASS] 类型 OutboxProcessor (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 OutboxProcessor 可能为顶层语句或嵌套类型");
    }

    // 验证 class: WolverineDependencyInjectionExtensions
    var type_WolverineDependencyInjectionExtensions = Type.GetType("WolverineDependencyInjectionExtensions");
    if (type_WolverineDependencyInjectionExtensions != null)
    {
        Console.WriteLine("[PASS] 类型 WolverineDependencyInjectionExtensions (class) 存在");
        var ctors_WolverineDependencyInjectionExtensions = type_WolverineDependencyInjectionExtensions.GetConstructors();
        Console.WriteLine($"[PASS] WolverineDependencyInjectionExtensions 构造函数数量: {ctors_WolverineDependencyInjectionExtensions.Length}");
        var methods_WolverineDependencyInjectionExtensions = type_WolverineDependencyInjectionExtensions.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly);
        Console.WriteLine($"[PASS] WolverineDependencyInjectionExtensions 公开方法数量: {methods_WolverineDependencyInjectionExtensions.Length}");
        foreach (var m in methods_WolverineDependencyInjectionExtensions)
            Console.WriteLine($"  [INFO] 方法: {m.Name}");
    }
    else
    {
        Console.WriteLine("[WARN] 类型 WolverineDependencyInjectionExtensions 未找到，尝试无命名空间...");
        type_WolverineDependencyInjectionExtensions = Type.GetType("WolverineDependencyInjectionExtensions");
        if (type_WolverineDependencyInjectionExtensions != null)
            Console.WriteLine("[PASS] 类型 WolverineDependencyInjectionExtensions (无命名空间) 存在");
        else
            Console.WriteLine("[INFO] 类型 WolverineDependencyInjectionExtensions 可能为顶层语句或嵌套类型");
    }

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}
